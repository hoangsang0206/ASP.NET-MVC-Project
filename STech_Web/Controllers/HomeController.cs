using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data.Entity.Core.Metadata.Edm;
using System.Web.Management;
using Microsoft.Owin.Security.Provider;

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
            List<ProductOutStanding> laptopOSD = productOutStandings.Where(t => t.Product.CateID == "laptop").ToList();
            List<ProductOutStanding> laptopGamingOSD = productOutStandings.Where(t => t.Product.CateID == "laptop-gaming").ToList();
            List<Slider> sliders = db.Sliders.ToList();
            List<Banner1> banner1 = db.Banner1.ToList();
            List<Brand> brands = db.Brands.Where(t => t.ParentBrandID == null).ToList();
            List<Category> categories = db.Categories.ToList();

            //--Countdown -----
            Countdown countdown = GetCountdown();
            ViewBag.CurrentDate = DateTime.Now;
            ViewBag.StartDate = countdown.startDate;
            ViewBag.EndDate = countdown.endDate;
            //----------
            ViewBag.Sales = sales;
            ViewBag.LaptopOSD = laptopOSD;
            ViewBag.LaptopGamingOSD = laptopGamingOSD;
            ViewBag.Sliders = sliders;
            ViewBag.Banner1 = banner1;
            ViewBag.Brands = brands;
            ViewBag.Categories = categories.OrderBy(c => c.Sort);
            ViewBag.ActiveBotNav = "home";

            //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;

            return View();
        }

        //----Countdown ------------------------------
        [HttpGet]
        public ActionResult Countdown() 
        {
            Countdown countdown = GetCountdown();
            if(DateTime.Now >= countdown.startDate && DateTime.Now <= countdown.endDate)
            {
                TimeSpan remainingTime = calculateRemainingTime(countdown.endDate);
                if (remainingTime.Days == 0 && remainingTime.Hours == 0 
                    && remainingTime.Minutes == 0 && remainingTime.Seconds == 0)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    times = remainingTime
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        //--Get countdown time
        Countdown GetCountdown()
        {
            string filePath = Server.MapPath("~/DataFiles/countdown.json");
            string json = System.IO.File.ReadAllText(filePath);
            Countdown countdown = JsonConvert.DeserializeObject<Countdown>(json);
            
            return countdown;
        }

        //---Calculate remaining time --------------
        private TimeSpan calculateRemainingTime(DateTime endDate)
        {
            return endDate - DateTime.Now;
        }


        //--------------------------------
        public ActionResult About() 
        {
            return View();
        }
    }
}