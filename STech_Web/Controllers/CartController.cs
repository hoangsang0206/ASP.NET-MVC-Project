using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using System.Runtime.ConstrainedExecution;
using System.Web.DynamicData;
using Microsoft.Owin;
using Newtonsoft.Json;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace STech_Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Cart> cartItems = new List<Cart>(); //Cart from database
            List<CartItem> cartCookie = new List<CartItem>();

            //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

            if (User.Identity.IsAuthenticated)
            {
                string userID = User.Identity.GetUserId();
                Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                string customerID;

                if(customer == null)
                {
                    customerID = addNewCustomer(db, userID);
                }

                cartCookie = getCartFromCookie();
                if(cartCookie.Count > 0)
                {
                    foreach (CartItem cartItem in cartCookie)
                    {
                        Cart cart = new Cart();
                        cart.ProductID = cartItem.ProductID;
                        cart.Quantity = cartItem.Quantity;
                        cart.UserID = User.Identity.GetUserId();

                        if(cart != null)
                        {
                            db.Carts.Add(cart);
                        }
                    }
                    db.SaveChanges();
                }

                cartItems = db.Carts.Where(t => t.UserID == userID).ToList();

                //Delete cookie
                Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(-10);
                ViewBag.CartCount = cartItems.Count;
                return View(cartItems);
            }
            else
            {
                cartCookie = getCartFromCookie();
                if(cartCookie.Count > 0)
                {
                    foreach (CartItem item in cartCookie)
                    {
                        Cart cart = new Cart();
                        cart.ProductID = item.ProductID;
                        cart.Quantity = item.Quantity;
                        cart.UserID = null;
                        if(cart != null)
                        {
                            cartItems.Add(cart);
                        }
                    }
                }

                var cartWithProduct = cartItems.Select(item => new
                {
                    item.Quantity,
                    Product = db.Products.FirstOrDefault(t => t.ProductID == item.ProductID)
                }).ToList();

                if(cartWithProduct.Count > 0)
                {
                    ViewBag.CartCount = cartWithProduct.Count;
                    return View(cartWithProduct);
                }
                else
                {
                    cartItems = new List<Cart>();
                    ViewBag.CartCount = cartItems.Count;
                    return View(cartItems);
                }    
            }
        }

        //Add product to cart
        [HttpPost]
        public ActionResult AddToCart(CartItem pro)
        {
            //Add data from cart to database when user logged in
            if (User.Identity.IsAuthenticated)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                string userID = User.Identity.GetUserId();
                List<Cart> userCart = db.Carts.Where(t => t.UserID == userID).ToList();

                if(userCart.Any(t => t.ProductID == pro.ProductID && t.UserID == userID))
                {
                    userCart.FirstOrDefault(t => t.ProductID == pro.ProductID && t.UserID == userID).Quantity += 1;
                    db.SaveChanges();
                }
                else
                {
                    Cart cartItem = new Cart();
                    cartItem.ProductID = pro.ProductID;
                    cartItem.Quantity = pro.Quantity;
                    cartItem.UserID = userID;
                    db.Carts.Add(cartItem);
                    db.SaveChanges();
                }   
                   
                return Json(new { success = true });
            }
            else //Add product to cart when user not logged in
            {    //data will save to cookie
                List<CartItem> cartItems = getCartFromCookie();

                //----------
                if(cartItems.Contains(pro))
                {
                    cartItems.FirstOrDefault(t => t.ProductID == pro.ProductID).Quantity += 1;
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        ProductID = pro.ProductID,
                        Quantity = pro.Quantity
                    });
                    
                }
                //--Save cart item list to cookie
                Response.Cookies["CartItems"].Value = JsonConvert.SerializeObject(cartItems);
                //Cookie will expire in 30 days from the date the new product is added
                Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
                return Json(new { success = true });
            }
        }

        //--Get cart items from Cookies
        private List<CartItem> getCartFromCookie()
        {
            List<CartItem> cartItems = new List<CartItem>();
            if (Request.Cookies["CartItems"] != null)
            {
                string cookieValue = Request.Cookies["CartItems"].Value;
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
            }

            return cartItems;
        }

        //--Add new customer ---------------
        public string addNewCustomer(DatabaseSTechEntities db, string userName)
        {
            Customer customer = new Customer();
            int customersCount = db.Customers.Count();
            string customerID = "KH" + (customersCount + 1).ToString().PadLeft(8, '0');

            customer = new Customer();
            customer.AccountID = userName;
            customer.CustomerID = customerID;
            db.Customers.Add(customer);

            return customerID;

        }

        //Update quantity
        public ActionResult UpdateQuantity(CartItem pro, int quantity)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }


            return Redirect("/cart");
        }

    }
}