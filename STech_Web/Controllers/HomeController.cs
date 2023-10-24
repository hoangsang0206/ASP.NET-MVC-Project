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
            List<Brand> brands = db.Brands.ToList();

            //--Countdown -----
            Countdown countdown = GetCountdown();
            ViewBag.CurrentDate = DateTime.Now;
            ViewBag.StartDate = countdown.startDate;

            //----------
            ViewBag.Sales = sales;
            ViewBag.LaptopOutStandings = laptopOutStandings;
            ViewBag.Sliders = sliders;
            ViewBag.Banner1 = banner1;
            ViewBag.Brands = brands;
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
            if(DateTime.Now >= countdown.startDate)
            {
                TimeSpan remainingTime = calculateRemainingTime(countdown.endDate);

                return Json(new
                {
                    success = true,
                    days = remainingTime.Days,
                    hours = remainingTime.Hours,
                    minutes = remainingTime.Minutes,
                    seconds = remainingTime.Seconds
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false });
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