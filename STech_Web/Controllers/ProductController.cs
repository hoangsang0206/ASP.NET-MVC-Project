using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace STech_Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Detail(string id)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.Where(t => t.ProductID == id)
                .FirstOrDefault();

            Category cate = db.Categories.Where(t => t.CateID == product.CateID)
                .FirstOrDefault();

            List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
            breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
            breadcrumb.Add(new Breadcrumb(cate.CateName, "/collections/"+cate.CateID+""));
            breadcrumb.Add(new Breadcrumb(product.ProductName, ""));

            ViewBag.Title = product.ProductName;
            ViewBag.Breadcrumb = breadcrumb;

            return View(product);
        }
    }
}