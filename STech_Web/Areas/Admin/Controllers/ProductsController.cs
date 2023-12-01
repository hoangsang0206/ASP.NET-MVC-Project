using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class ProductsController : Controller
    {
        // GET: Admin/Products
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.OrderBy(t => t.Sort).ToList();
            List<Brand> brands = db.Brands.ToList();

            ViewBag.ActiveNav = "products";
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            return View();
        }

        //Product detail page
        public ActionResult Detail(string id)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == id);
                if(product == null)
                {
                    return Redirect("/admin/products");
                }

                List<Category> categories = db.Categories.OrderBy(t => t.Sort).ToList();
                List<Brand> brands = db.Brands.ToList();

                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View(product);
            }
            catch (Exception ex)
            {
                return Redirect("/admin/products");
            }
        }

        //Check valid value
        string checkValidValue(Product product, int quantity)
        {
            //Kiểm tra dữ liệu không để trống
            if (product.ProductID == null || product.ProductName == null || product.ImgSrc == null || product.Cost == null)
            {
                return "Vui lòng nhập đầy đủ thông tin.";
            }

            if (product.ProductID.Contains(" "))
            {
                return "Mã sản phẩm không được chứa khoảng trắng.";
            }

            //Số lượng và thời gian bảo hành >= 0
            if (quantity < 0)
            {
                return "Số lượng phải lớn hơn 0.";
            }

            if (product.Warranty < 0)
            {
                return "Thời gian bảo hành không được nhỏ hơn 0.";
            }

            //Giá không được bé hơn 0 
            if(product.Cost < 0)
            {
                return "Giá của sản phẩm phải lớn hơn 0.";
            }

            //Giá khuyến mãi < giá gốc
            if (product.Price >= product.Cost)
            {
                return "Giá khuyến mãi phải thấp hơn giá gốc.";
            }

            return null;
        }


        //Add new product --------
        [HttpPost]
        public JsonResult AddProduct(Product product, int quantity)
        {
            if(ModelState.IsValid)
            {
                string checkValue = checkValidValue(product, quantity);
                if (checkValue != null)
                {
                    return Json(new { success = false, error = checkValue });
                }

                //--------------
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Product> products = db.Products.ToList();
                //Kiểm tra xem sản phẩm đã tồn tại chưa
                Product pro = products.FirstOrDefault(t => t.ProductID == product.ProductID);
                if (pro != null)
                {
                    return Json(new { success = false, error = "Sản phẩm này đã tồn tại." });
                }

                //---
                if (product.Price == null)
                {
                    product.Price = product.Cost;
                }
                
                //---
                if(product.CateID == null)
                {
                    product.CateID = "khac";
                }
                if(product.BrandID == null)
                {
                    product.BrandID = "khac";
                }

                //---
                db.Products.Add(product);

                //---
                WareHouse wh = new WareHouse();
                wh.ProductID = product.ProductID;
                wh.Quantity = quantity;
                db.WareHouses.Add(wh);
                db.SaveChanges();
                //---
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

        //Update product
        [HttpPost]
        public JsonResult UpdateProduct(Product product, int quantity)
        {
            if(ModelState.IsValid)
            {
                string checkValue = checkValidValue(product, quantity);
                if (checkValue != null)
                {
                    return Json(new { success = false, error = checkValue });
                }

                //--------------
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Product> products = db.Products.ToList();
                //Kiểm tra xem sản phẩm có tồn tại không
                Product pro = products.FirstOrDefault(t => t.ProductID == product.ProductID);
                if (pro == null)
                {
                    return Json(new { success = false, error = "Sản phẩm này không tồn tại." });
                }

                //------------
                if (product.Price == null)
                {
                    product.Price = product.Cost;
                }
                pro.ProductName = product.ProductName;
                pro.ImgSrc = product.ImgSrc;
                pro.ImgSrc1 = product.ImgSrc1;
                pro.ImgSrc2 = product.ImgSrc2;
                pro.ImgSrc3 = product.ImgSrc3;
                pro.ImgSrc4 = product.ImgSrc4;
                pro.ImgSrc5 = product.ImgSrc5;
                pro.ImgSrc6 = product.ImgSrc6;
                pro.ImgSrc7 = product.ImgSrc7;
                pro.Cost = product.Cost;
                pro.Price = product.Price;
                pro.CateID = product.CateID;
                pro.BrandID = product.BrandID;
                pro.Warranty = product.Warranty;
                pro.ManufacturingDate = product.ManufacturingDate;
                pro.Type = product.Type;
                pro.Description = product.Description;

                //--------------
                WareHouse wh = pro.WareHouse;
                wh.Quantity = quantity;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }
    }
}