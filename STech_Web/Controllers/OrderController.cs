using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using Microsoft.AspNet.Identity;
using System.Web.Management;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using STech_Web.Identity;
using System.Globalization;
using System.Text;

namespace STech_Web.Controllers
{
    [UserAuthorization]
    public class OrderController : Controller
    {
        //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

        // GET: Order
        public ActionResult OrderInfo()
        {
            var base64String = Request.Cookies["OrderTemp"]?.Value;
            OrderTemp orderTemp = new OrderTemp();

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);

                if(orderTemp == null)
                {
                    return Redirect("/cart");
                }
            }
            else
            {
                return Redirect("/cart");
            }

            //--------------------
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(User.Identity.GetUserId());

            ViewBag.User = user;
            ViewBag.Order = orderTemp;

            //----------------------
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            string userID = User.Identity.GetUserId();
            List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();
            decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);

            ViewBag.TotalPrice = totalPrice.ToString("##,###", cul);

            return View();
        }

        public ActionResult MakeOrder()
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

        //Check order infomation 
        [HttpPost]
        public ActionResult CheckOrderInfo(string gender, string customerName, string customerPhone, string address, string shipMethod, string note)
        {
            //Không được để trống thông tin
            if (String.IsNullOrEmpty(gender) || String.IsNullOrEmpty(customerName) || String.IsNullOrEmpty(customerPhone) || String.IsNullOrEmpty(shipMethod))
            {
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
            }

            //Kiểm tra số điện thoại
            if (!(customerPhone.StartsWith("0")) || customerPhone.Length != 10 || !Regex.IsMatch(customerPhone, @"^[0-9]+$"))
            {
                string err = "Số điện thoại không hợp lệ.";
                return Json(new { success = false, error = err });
            }

            //Kiểm tra địa chỉ nhận hàng nếu chọn COD
            if(shipMethod == "COD")
            {
                if(String.IsNullOrEmpty(address))
                {
                    string err = "Vui lòng nhập địa chỉ nhận hàng.";
                    return Json(new { success = false, error = err });
                }
            }

            //Tạo đơn hàng nháp
            OrderTemp orderTemp = new OrderTemp(shipMethod, note);
            var orderTempJson = JsonConvert.SerializeObject(orderTemp);
            var bytesToEncode = Encoding.UTF8.GetBytes(orderTempJson);
            var base64String = Convert.ToBase64String(bytesToEncode);

            //--Save cart item list to cookie
            Response.Cookies["OrderTemp"].Value = base64String;
            //Cookie will expire in 30 days from the date the new product is added
            Response.Cookies["OrderTemp"].Expires = DateTime.Now.AddMinutes(30);

            return Json( new { success = true });
        }
    }
}