using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace STech_Web.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index()
        {

            ViewBag.ActiveNav = "orders";
            return View();
        }
    }
}