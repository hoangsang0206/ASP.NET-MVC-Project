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

        //In hóa đơn ----------------
        public ActionResult PrintOrder()
        {
           

            return View();
        }

        //Cập nhật trạng thái thanh toán -----------
        [HttpPost]
        public JsonResult UpdatePaymentStatus(string orderID = "")
        {

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