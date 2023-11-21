using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using Microsoft.Owin.Security;
using System.Security;
using STech_Web.Identity;
using Microsoft.AspNet.Identity;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class OrdersController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.Where(t => t.Status == "Chờ xác nhận").ToList();

            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;
            ViewBag.ActiveNav = "orders";
            return View(orders);
        }

        //Đếm số hóa đơn mới chờ xác nhận---
        [HttpGet]
        public JsonResult CountNewOrder()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> order = db.Orders.Where(t => t.Status == "Chờ xác nhận").ToList();
            return Json(new { count = order.Count }, JsonRequestBehavior.AllowGet);
        }

        //In hóa đơn ----------------
        public ActionResult PrintOrder()
        {
           

            return View();
        }

        //Xác nhận đã thanh toán -----------
        [HttpPost]
        public JsonResult AcceptPaid(string orderID = "")
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);
            if(order != null)
            {
                order.PaymentStatus = "Thanh toán thành công";
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        //Xác nhận/hủy đơn hàng --------------
        [HttpPost]
        public JsonResult UpdateStatus(string orderID = "", string type = "")
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);

            if(order == null)
            {
                return Json(new { success = false });
            }

            if(type == "accept")
            {
                order.Status = "Đã xác nhận";
            }
            else
            {
                order.Status = "Đã hủy";
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        //Tạo đơn hàng ---------------
        public JsonResult CreateOrder(string customerID, List<string> productID, string paymentMethod)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Customer customer = db.Customers.FirstOrDefault(c => c.CustomerID == customerID);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }         
        }

        //--Tạo khách hàng mới ---------------
        public string addNewCustomer(DatabaseSTechEntities db, string userID)
        {

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            STech_Web.Models.Customer customer = new STech_Web.Models.Customer();
            List<STech_Web.Models.Customer> customers = db.Customers.OrderByDescending(t => t.CustomerID).ToList();
            int customerNumber = 1;
            if (customers.Count > 0)
            {
                customerNumber = int.Parse(customers[0].CustomerID.Substring(2)) + 1;
            }

            string customerID = "KH" + customerNumber.ToString().PadLeft(4, '0');

            customer = new STech_Web.Models.Customer();
            customer.AccountID = userID;
            customer.CustomerID = customerID;
            customer.CustomerName = user.UserFullName;
            customer.Address = user.Address;
            customer.Phone = user.PhoneNumber;
            customer.Email = user.Email;
            customer.DoB = user.DOB;
            customer.Gender = user.Gender;

            db.Customers.Add(customer);
            db.SaveChanges();

            return customerID;

        }

        //Tăng/giảm số lượng sản phẩm trong đơn hàng
        [HttpPost]
        public JsonResult UpdateProductQty(string productID, int qty)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
            if(product == null)
            {
                return Json(new { success = false });
            }

            if (qty <= 0) qty = 1;
            if (product.WareHouse.Quantity < qty) qty = (int)product.WareHouse.Quantity;

            return Json(new { success = true, quantity = qty, total = qty * product.Price });
        }

        //Xóa đơn hàng ---------------
        [HttpPost]
        public JsonResult DeleteOrder(string orderID = "")
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);
                if(order == null)
                {
                    return Json(new { success = false, error = "Đã xẩy ra lỗi" });
                }

                List<OrderDetail> orderDetails = order.OrderDetails.ToList();
                //Cập nhật lại số lượng sản phẩm
                foreach (OrderDetail orderDetail in orderDetails)
                {
                    WareHouse wh = orderDetail.Product.WareHouse;
                    wh.Quantity += orderDetail.Quantity;
                }
                db.OrderDetails.RemoveRange(orderDetails);
                db.Orders.Remove(order);
                db.SaveChanges(); 

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Đã xẩy ra lỗi" });
            }       
        }
    }
}