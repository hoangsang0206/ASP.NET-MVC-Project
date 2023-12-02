using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Identity;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            ViewBag.ActiveNav = "users";
            return View();
        }

        //Kiểm tra SDT đã tồn tại trong Identity User
        private bool CheckUserPhoneExist(string phone, string userID)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var allUsers = userManager.Users.ToList();

            if (allUsers.Any(t => t.Id != userID && t.PhoneNumber == phone))
            {
                return true;
            }

            return false;
        }
        //Kiểm tra Email đã tồn tại trong Identity User
        private bool CheckUserEmailExist(string email, string userID)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var allUsers = userManager.Users.ToList();

            if (allUsers.Any(t => t.Id != userID && t.Email == email))
            {
                return true;
            }

            return false;
        }

        //Kiểm tra SDT đã tồn tại trong bảng Customer
        private bool checkCustomerPhoneExist(string phone)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Customer> customers = db.Customers.ToList();

            if(customers.Count > 0 && customers.Any(c => c.Phone == phone))
            {
                return true;
            }

            return false;
        }

        //Kiểm tra Email đã tồn tại trong bảng Customer
        private bool checkCustomerEmailExist(string email) 
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Customer> customers = db.Customers.ToList();

            if (customers.Count > 0 && customers.Any(c => c.Email == email))
            {
                return true;
            }

            return false;
        }

        //Tạo tài khoản

        //Tạo khách hàng
        [HttpPost]
        public JsonResult CreateCustomer(Customer cus)
        {
            try
            {
                if(checkCustomerPhoneExist(cus.Phone))
                {
                    return Json(new { success = false, error = "Số điện thoại này đã tồn tại. " });
                }
                if(checkCustomerEmailExist(cus.Email))
                {
                    return Json(new { success = false, error = "Email này đã tồn tại. " });
                }

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
                return Json(new { success = false });
            }         
        }
    }
}