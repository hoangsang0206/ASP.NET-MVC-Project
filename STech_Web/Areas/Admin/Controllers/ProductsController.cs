using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class ProductsController : Controller
    {
        // GET: Admin/Products
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();
            List<Brand> brands = db.Brands.ToList();

            ViewBag.ActiveNav = "products";
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            return View();
        }

        //Product detail page
        public ActionResult Detail(string id)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.FirstOrDefault(t => t.ProductID == id);
            List<Category> categories = db.Categories.ToList();
            List<Brand> brands = db.Brands.ToList();
            ProductImgDetail imgDetail = product.ProductImgDetails.FirstOrDefault();

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            ViewBag.ImgDetail = imgDetail;
            return View(product);
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


        //Update product imgage detail -------------------------------------------------
        public void updateProductImages(DatabaseSTechEntities db, ProductImgDetail imgDetail, string productID, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
        {
            if (imgDetail == null)
            {
                imgDetail = new ProductImgDetail();
                imgDetail.ProductID = productID;
                imgDetail.ImgSrc1 = imgSrc1;
                imgDetail.ImgSrc2 = imgSrc2;
                imgDetail.ImgSrc3 = imgSrc3;
                imgDetail.ImgSrc4 = imgSrc4;

                db.ProductImgDetails.Add(imgDetail);
                return;
            }

            imgDetail.ImgSrc1 = imgSrc1;
            imgDetail.ImgSrc2 = imgSrc2;
            imgDetail.ImgSrc3 = imgSrc3;
            imgDetail.ImgSrc4 = imgSrc4;
        }

        //Add new product --------
        [HttpPost]
        public ActionResult AddProduct(Product product, int quantity, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
        {
            string checkValue = checkValidValue(product, quantity);
            if(checkValue != null)
            {
                return Json(new { success = false, error = checkValue });
            }

            //--------------
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.ToList();
            //Kiểm tra xem sản phẩm đã tồn tại chưa
            Product pro = products.FirstOrDefault(t => t.ProductID == product.ProductID);
            if(pro != null)
            {
                return Json(new { success = false, error = "Sản phẩm này đã tồn tại." });
            }

            //---
            if (product.Price == null)
            {
                product.Price = product.Cost;
                product.Cost = null;
            }
            //---
            db.Products.Add(product);

            //---
            WareHouse wh = new WareHouse();
            wh.ProductID = product.ProductID;
            wh.Quantity = quantity;
            db.WareHouses.Add(wh);

            //---
            ProductImgDetail imgDetail = pro.ProductImgDetails.FirstOrDefault();
            updateProductImages(db, imgDetail, pro.ProductID, imgSrc1, imgSrc2, imgSrc3, imgSrc4);
            //---
            db.SaveChanges();
            //---
            return Json(new { success = true });
        }

        //Update product
        [HttpPost]
        public ActionResult UpdateProduct(Product product, int quantity, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
        {
            string checkValue = checkValidValue(product, quantity);
            if(checkValue != null)
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
                product.Cost = null;
            }
            pro.ProductName = product.ProductName;
            pro.ImgSrc = product.ImgSrc;
            pro.Cost = product.Cost;
            pro.Price = product.Price;
            pro.CateID = product.CateID;
            pro.BrandID = product.BrandID;
            pro.Warranty = product.Warranty;

            //--------------
            WareHouse wh = pro.WareHouse;
            wh.Quantity = quantity;

            //--------------
            ProductImgDetail imgDetail = pro.ProductImgDetails.FirstOrDefault();
            updateProductImages(db, imgDetail, pro.ProductID, imgSrc1, imgSrc2, imgSrc3, imgSrc4);

            //----------
            db.SaveChanges();
            return Json(new { success = true });
        }

        //Delete product
        [HttpPost]
        public ActionResult DeleteProduct(string id)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.FirstOrDefault(t => t.ProductID == id);
            if (product == null)
            {
                return Json(new { success = false, error = "Sản phẩm này không tồn tại." });
            }

            db.Products.Remove(product);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}