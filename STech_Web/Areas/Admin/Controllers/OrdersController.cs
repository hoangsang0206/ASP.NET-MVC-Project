using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using iTextSharp;
using iTextSharp.text.pdf;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class OrdersController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index()
        {
            ViewBag.ActiveNav = "orders";
            return View();
        }

        //In hóa đơn ----------------
        public ActionResult PrintOrder()
        {
           

            return View();
        }
    }
}