using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        public ActionResult Index()
        {
            ViewBag.ActiveNav = "sale";
            return View();
        }
    }
}