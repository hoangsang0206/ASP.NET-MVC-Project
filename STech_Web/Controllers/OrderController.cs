using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using Microsoft.AspNet.Identity;

namespace STech_Web.Controllers
{
    [UserAuthorization]
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);

            if (customer == null)
            {
                addNewCustomer(db, userID);
            }

            return View();
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
    }
}