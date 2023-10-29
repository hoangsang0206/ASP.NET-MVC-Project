﻿using System;
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
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.Text;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;

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
                List<CartTemp> cartTemp = new List<CartTemp>();
                if (cartCookie.Count > 0)
                {
                    foreach (CartItem item in cartCookie)
                    {
                        Product product = new Product();
                        int quantity = item.Quantity;
                        product = db.Products.FirstOrDefault(t => t.ProductID == item.ProductID);

                        CartTemp cTemp = new CartTemp(product, quantity);
                        cartTemp.Add(cTemp);
                    }
                }

                if(cartTemp.Count > 0)
                {
                    ViewBag.CartCount = cartTemp.Count;
                    return View(cartTemp);
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
        public ActionResult AddToCart(CartItem cart)
        {
            //Add data from cart to database when user logged in
            if (User.Identity.IsAuthenticated)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                string userID = User.Identity.GetUserId();
                List<Cart> userCart = db.Carts.Where(t => t.UserID == userID).ToList();

                Cart existCart = userCart.FirstOrDefault(t => t.ProductID == cart.ProductID);
                if (existCart != null)
                {
                    existCart.Quantity += 1;
                    db.SaveChanges();
                }
                else
                {
                    Cart cartItem = new Cart();
                    cartItem.ProductID = cart.ProductID;
                    cartItem.Quantity = cart.Quantity;
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
                CartItem cartItem = cartItems.FirstOrDefault(t => t.ProductID == cart.ProductID);
                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        ProductID = cart.ProductID,
                        Quantity = cart.Quantity
                    });
                    
                }
                var cartJson = JsonConvert.SerializeObject(cartItems);
                var bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
                var base64String = Convert.ToBase64String(bytesToEncode);

                //--Save cart item list to cookie
                Response.Cookies["CartItems"].Value = base64String;
                //Cookie will expire in 30 days from the date the new product is added
                Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
                return Json(new { success = true });
            }
        }

        //--Delete item from cart
        public ActionResult DeleteCartItem(int line = 0)
        {
            if (line > 0)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Cart> cartList = new List<Cart>();
                //Add to cart when user logged in
                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();
                    cartList = db.Carts.Where(t => t.UserID == userID).ToList();
                    if (line > cartList.Count)
                    {
                        line = cartList.Count;
                    }
                    db.Carts.Remove(cartList[line - 1]);
                    db.SaveChanges();
                }
                else
                {   //Add to cart when user not logged in
                    List<CartItem> cartCookie = getCartFromCookie();
                    if(line > cartCookie.Count)
                    {
                        line = cartCookie.Count;
                    }

                    cartCookie.RemoveAt(line - 1);

                    var cartJson = JsonConvert.SerializeObject(cartCookie);
                    var bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
                    var base64String = Convert.ToBase64String(bytesToEncode);
                    string json = JsonConvert.SerializeObject(cartCookie);
                    Response.Cookies["CartItems"].Value = base64String;
                    //Cookie will expire in 30 days from the date the new product is added
                    Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
                }
                
            }

            return Redirect("/cart");
        }

        //--Get cart items from Cookies
        private List<CartItem> getCartFromCookie()
        {
            List<CartItem> cartItems = new List<CartItem>();
            var base64String = Request.Cookies["CartItems"]?.Value;

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var cartItemsJson = Encoding.UTF8.GetString(bytesToDecode);
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
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

        //Update cart item quantity
        public ActionResult UpdateQuantity(string productID, string updateType)
        {
            //Update cart item quantity when user logged in
            if(User.Identity.IsAuthenticated)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                string userID = User.Identity.GetUserId();
                Cart cart = db.Carts.FirstOrDefault(t => t.UserID == userID && t.ProductID == productID);

                if(cart != null)
                {
                    if (updateType == "increase")
                    {
                        cart.Quantity += 1;
                    }
                    else if (updateType == "decrease")
                    {
                        cart.Quantity -= 1;
                        if (cart.Quantity <= 0) cart.Quantity = 1;
                    }
                    db.Carts.AddOrUpdate(cart);
                    db.SaveChanges();
                }       
            }
            else
            {   //Update cart item quantity when user not logged in
                List<CartItem> cartCookie = getCartFromCookie();
                CartItem cartCCItem =  cartCookie.FirstOrDefault(t => t.ProductID == productID);
                if (cartCCItem != null)
                {
                    if (updateType == "increase" && cartCCItem != null)
                    {
                        cartCCItem.Quantity += 1;
                    }
                    else if (updateType == "decrease" && cartCCItem != null)
                    {
                        cartCCItem.Quantity -= 1;
                        if (cartCCItem.Quantity <= 0) cartCCItem.Quantity = 1;
                    }
                    cartCookie.Remove(cartCCItem);
                    cartCookie.Add(cartCCItem);

                    var cartJson = JsonConvert.SerializeObject(cartCookie);
                    var bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
                    var base64String = Convert.ToBase64String(bytesToEncode);
                    string json = JsonConvert.SerializeObject(cartCookie);
                    Response.Cookies["CartItems"].Value = base64String;
                    //Cookie will expire in 30 days from the date the new product is added
                    Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
                }
            }

            return Redirect("/cart");
        }

        //Count item in cart
        [HttpPost]
        public ActionResult CartCount()
        {
            //Count item in cart when user logged in
            if(User.Identity.IsAuthenticated)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                string userID = User.Identity.GetUserId();
                int cartCount = db.Carts.Where(t => t.UserID == userID).Count();

                return Json(new { count = cartCount });
            }
            else
            {   //Count item in cart when user not logged in
                List<CartItem> cartItems = getCartFromCookie();
                int cartCount = cartItems.Count();

                return Json(new { count = cartCount });
            }
        }

    }
}