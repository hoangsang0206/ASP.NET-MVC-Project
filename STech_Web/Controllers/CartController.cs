using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;

namespace STech_Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {

            //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;
            return View();
        }

        //Add product to cart when user not logged in
        //data will add to JSON
        [HttpPost]
        public ActionResult AddToCart(ProductInCart pro)
        {
            CartTemp cartTemp = new CartTemp();
            if (pro.ProductID != null && pro.ProductID.Length > 0)
            {
                cartTemp.AddToCart(pro);
                return Json(new { success = true }) ;
            }

            return Json(new { success = false, errror = "Thêm thất bại." });
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

        //Add data from cart to database when user logged in
        [UserAuthorization, HttpPost]
        public ActionResult AddToCartWhenLoggedIn(ProductInCart pro, string userName)
        {
            if (pro.ProductID != null && pro.ProductID.Length > 0)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
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

            return Json(new { success = false, error = "Thêm thất bại." });
        }

        //Add all data from cartTemp to database when user logged in
        [UserAuthorization, HttpPost]
        public ActionResult AddAllToCart(string userName) 
        {
            CartTemp cartTemp = new CartTemp();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userName);
            if(customer == null)
            {
                cartTemp.addAllToDB(addNewCustomer(db, userName));
            }
            else
            {
                cartTemp.addAllToDB(customer.CustomerID);
            }
           

            return Redirect("/cart");
        }

        //Update quantity when user not logged in
        public ActionResult UpdateQuantity(ProductInCart pro, int quantity)
        {
            CartTemp cartTemp = new CartTemp();
            if (quantity <= 0)
            {
                quantity = 1;
            }

            cartTemp.updateQuantity(pro, quantity);

            return Redirect("/cart");
        }

        //Update quantity when user logged in
        public ActionResult UpdateQuantityWhenLoggedIn(ProductInCart pro, string customerID, int quantity)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Cart cartItem = db.Carts.FirstOrDefault(t => t.CustomerID == customerID && t.ProductID == pro.ProductID);
           
            cartItem.Quantity = quantity;
            db.SaveChanges();

            return Redirect("/cart");
        }
    }
}