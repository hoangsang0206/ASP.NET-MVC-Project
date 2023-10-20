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

        //Add new product --------
        [HttpPost]
        public ActionResult AddProduct(Product product, int quantity, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
        {
            //Kiểm tra dữ liệu không để trống
            if (product.ProductID == null || product.ProductName == null || product.ImgSrc == null || product.Cost == null)
            {
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
            }

            if (product.ProductID.Contains(" "))
            {
                return Json(new { success = false, error = "Mã sản phẩm không được chứa khoảng trắng." });
            }
            
            //Số lượng và thời gian bảo hành >= 0
            if (quantity < 0)
            {
                return Json(new { success = false, error = "Số lượng phải lớn hơn 0." });
            }

            if(product.Warranty < 0)
            {
                return Json(new { success = false, error = "Thời gian bảo hành không được nhỏ hơn 0." });
            }

            //Giá khuyến mãi < giá gốc
            if(product.Price >= product.Cost)
            {
                return Json(new { success = false, error = "Giá khuyến mãi phải thấp hơn giá gốc." });
            }

            //--------------
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            //Kiểm tra xem sản phẩm đã tồn tại chưa
            List<Product> products = db.Products.ToList();
            foreach(Product p in products) 
            {
                if (p.ProductID == product.ProductID)
                {
                    return Json(new { success = false, error = "Sản phẩm này đã tồn tại." });
                }
            }
            
            //---
            WareHouse wh = new WareHouse();
            wh.ProductID = product.ProductID;
            wh.Quantity = quantity;
            //---
            if (imgSrc1 != null || imgSrc1.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc1;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc2 != null || imgSrc2.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc2;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc3 != null || imgSrc3.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc3;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc4 != null || imgSrc4.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc4;

                db.ProductImgDetails.Add(imgDetail);
            }

            //---
            if(product.Price == null)
            {
                product.Price = product.Cost;
                product.Cost = null;
            }
            //---
            db.Products.Add(product);
            db.WareHouses.Add(wh);
            db.SaveChanges();
            //---
            return Json(new { success = true });
        }
    }
}