using Microsoft.Owin.Security;
using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using STech_Web.Filters;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class CategoriesController : Controller
    {
        // GET: Admin/Categories
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();

            ViewBag.ActiveNav = "categories";
            return View(categories);
        }

        //Thêm danh mục
        public ActionResult AddCategory(Category cate)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();

            //Kiểm tra category đã tồn tại chưa
            foreach (Category c in categories)
            {
                if(c.CateID == cate.CateID)
                {
                    string err = "Danh mục này đã tồn tại";
                    return Json(new { success = false, error = err });
                }    
            }

            db.Categories.Add(cate);
            db.SaveChanges();

            return Json(new { success = true});
        }

        //Xóa danh mục
        public ActionResult DeleteCategory(string cateID) 
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();

            //Kiểm tra xem danh mục này có sản phẩm không 
            List<Product> products = db.Products.Where(t => t.CateID == cateID).ToList();
            if(products.Count > 0)
            {
                string error = "Danh mục này đã có " + products.Count + " sản phẩm, không thể xóa";
                return Json(new { success = false, err = error });

            }

            foreach (Category c in categories)
            {
                if(c.CateID == cateID)
                {
                    db.Categories.Remove(c);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false , err = "Xóa thất bại."});
        }

        //Sửa danh mục
        public ActionResult UpdateCategory(Category cate)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();

            foreach (Category c in categories)
            {
                if (c.CateID == cate.CateID)
                {
                    c.CateName = cate.CateName;
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, error = "Sửa thất bại" });
        }
    }
}