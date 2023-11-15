using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using iTextSharp;
using iTextSharp.text.pdf;
using STech_Web.Models;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using Microsoft.Owin.Security;

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

        //Xác nhận/hủy đơn hàng
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
        public JsonResult CreateOrder()
        {

            return Json(new { success = false });
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