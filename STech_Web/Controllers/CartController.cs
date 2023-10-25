using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace STech_Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {

            //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;
            return View();
        }

        //Add product to cart
        [HttpPost]
        public ActionResult AddToCart(string productID = "", int quantity = 1)
        {
            return Json(new { success = true });
        }


    }
}