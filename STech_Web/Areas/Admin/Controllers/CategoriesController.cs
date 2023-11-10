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
            if(ModelState.IsValid)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Category> categories = db.Categories.ToList();

                //Kiểm tra category đã tồn tại chưa
                Category category = db.Categories.FirstOrDefault(t => t.CateID == cate.CateID);

                if (category != null)
                {
                    return Json(new { success = false, error = "Danh mục này đã tồn tại." });
                }

                db.Categories.Add(cate);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

        //Xóa danh mục
        public ActionResult DeleteCategory(string cateID) 
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();

            //Kiểm tra xem danh mục này có sản phẩm không 
            List<Product> products = db.Products.Where(t => t.CateID == cateID).ToList();
            if(products.Count > 0)
            {
                string error = "Danh mục này đã có " + products.Count + " sản phẩm, không thể xóa";
                return Json(new { success = false, err = error });

            }

            //Kiểm tra xem danh mục có tồn tại không
            Category category = db.Categories.FirstOrDefault(t => t.CateID == cateID);
            if (category == null)
            {
                return Json(new { success = false, error = "Danh mục này không tồn tại." });
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Json(new { success = true });
        }

        //Sửa danh mục
        public ActionResult UpdateCategory(Category cate)
        {
            if(ModelState.IsValid)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Category category = db.Categories.FirstOrDefault(t => t.CateID == cate.CateID);

                //Kiểm tra xem danh mục có tồn tại không
                if (category == null)
                {
                    return Json(new { success = false, error = "Danh mục này không tồn tại." });
                }

                //------
                if (cate.CateName == null || cate.CateName.Length <= 0)
                {
                    return Json(new { success = false, error = "Tên danh mục không được để trống." });
                }

                category.CateName = cate.CateName;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }
    }
}