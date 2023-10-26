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
                cartItems = db.Carts.Where(t => t.Customer.AccountID == User.Identity.Name).ToList();
                cartCookie = getCartFromCookie();

                foreach(CartItem cartItem in cartCookie)
                {
                    Cart cart = new Cart();
                    cart.ProductID = cartItem.ProductID;
                    cart.Quantity = cartItem.Quantity;
                }
            }
            else
            {
                cartCookie = getCartFromCookie();

                foreach(var item in cartCookie)
                {
                    Cart cartItem = new Cart();
                    cartItem.ProductID = item.ProductID;
                    cartItem.Quantity = item.Quantity;
                    cartItem.CustomerID = "undefined";
                    cartItems.Add(cartItem);
                }
            }

            
            return View();
        }

        //data will add to JSON
        [HttpPost]
        public ActionResult AddToCart(CartItem pro)
        {
            //Add data from cart to database when user logged in
            if (User.Identity.IsAuthenticated)
            {

                if (pro.ProductID != null && pro.ProductID.Length > 0)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    string userName = User.Identity.Name;
                    Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userName);

                    Cart cartItem = new Cart();
                    cartItem.ProductID = pro.ProductID;
                    cartItem.Quantity = pro.Quantity;

                    if (customer == null)
                    {
                        cartItem.CustomerID = addNewCustomer(db, userName);
                    }
                    else
                    {
                        cartItem.CustomerID = customer.CustomerID;
                    }

                    db.Carts.Add(cartItem);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            else //Add product to cart when user not logged in
            {    //data will save to cookie
                List<CartItem> cartItems = getCartFromCookie();

                //----------
                if (pro.ProductID != null && pro.ProductID.Length > 0)
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

            return Json(new { success = false, errror = "Thêm thất bại." });
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

        //Update quantity when user not logged in
        public ActionResult UpdateQuantity(CartItem pro, int quantity)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }


            return Redirect("/cart");
        }

        //Update quantity when user logged in
        public ActionResult UpdateQuantityWhenLoggedIn(CartItem pro, string customerID, int quantity)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Cart cartItem = db.Carts.FirstOrDefault(t => t.CustomerID == customerID && t.ProductID == pro.ProductID);
           
            cartItem.Quantity = quantity;
            db.SaveChanges();

            return Redirect("/cart");
        }
    }
}