using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class DashboardController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.ToList();
            List<Category> categories = db.Categories.ToList();
            List<Brand> brands = db.Brands.ToList();
            
            ViewBag.productCount = products.Count;
            ViewBag.categoryCount = categories.Count;
            ViewBag.brandCount = brands.Count;
            ViewBag.ActiveNav = "dashboard";
            return View();
        }
    }
}