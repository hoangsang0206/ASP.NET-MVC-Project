using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            return View();
        }

        //Tạo tài khoản

        //Tạo khách hàng
        [HttpPost]
        public JsonResult CreateCustomer(Customer cus)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Customer customer = new Customer();
                List<Customer> customers = db.Customers.OrderByDescending(t => t.CustomerID).ToList();

                int customerNumber = 1;
                if (customers.Count > 0)
                {
                    customerNumber = int.Parse(customers[0].CustomerID.Substring(2)) + 1;
                }

                string customerID = "KH" + customerNumber.ToString().PadLeft(4, '0');

                customer = new Customer();
                customer.AccountID = "";
                customer.CustomerID = customerID;
                customer.CustomerName = cus.CustomerName;
                customer.Address = cus.Address;
                customer.Phone = cus.Phone;
                customer.Email = cus.Email;
                customer.DoB = cus.DoB;
                customer.Gender = cus.Gender;

                db.Customers.Add(customer);
                db.SaveChanges();
                return Json(new { success = true }); 
            }
            catch(Exception ex)
            {
                return Json(new { success = true });
            }         
        }
    }
}