using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace STech_Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Sale> sales = db.Sales.ToList();
            List<ProductOutStanding> productOutStandings = db.ProductOutStandings.ToList();
            List<ProductOutStanding> laptopOutStandings = productOutStandings.Where(t => t.StandingType == "laptop").ToList();
            List<Slider> sliders = db.Sliders.ToList();
            List<Banner1> banner1 = db.Banner1.ToList();

            ViewBag.Sales = sales;
            ViewBag.LaptopOutStandings = laptopOutStandings;
            ViewBag.Sliders = sliders;
            ViewBag.Banner1 = banner1;

            //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;

            return View();
        }
    }
}