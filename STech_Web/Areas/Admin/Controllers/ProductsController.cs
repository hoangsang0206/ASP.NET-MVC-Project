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
            Product product = db.Products.Find(id);
            List<Category> categories = db.Categories.ToList();
            List<Brand> brands = db.Brands.ToList();

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
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
            if (imgSrc1.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc1;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc2.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc2;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc3.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc3;

                db.ProductImgDetails.Add(imgDetail);
            }
            if (imgSrc4.Length > 0)
            {
                ProductImgDetail imgDetail = new ProductImgDetail();
                imgDetail.ProductID = product.ProductID;
                imgDetail.ImgDetailSrc = imgSrc4;

                db.ProductImgDetails.Add(imgDetail);
            }

            //---
            db.SaveChanges();
            //---
            return Json(new { success = true });
        }

        //Update product imgage detail
        public void updateProductImages(DatabaseSTechEntities db, List<ProductImgDetail> imgDetails, string productID, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
        {
            if (imgDetails.Count == 0)
            {
                ProductImgDetail newImgDetail = new ProductImgDetail();
                newImgDetail.ProductID = productID;
                if(imgSrc1.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if(imgSrc2.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if(imgSrc3.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if(imgSrc4.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }

                if(newImgDetail.ImgDetailSrc == null || newImgDetail.ImgDetailSrc.Length <= 0)
                {
                    return;
                }

                db.ProductImgDetails.Add(newImgDetail);
            }
            if (imgDetails.Count == 1)
            {
                ProductImgDetail newImgDetail = new ProductImgDetail();
                newImgDetail.ProductID = productID;

                if (imgSrc1.Length > 0)
                {
                    imgDetails[0].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc2.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if (imgSrc3.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if (imgSrc4.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }

                if (newImgDetail.ImgDetailSrc == null || newImgDetail.ImgDetailSrc.Length <= 0)
                {
                    return;
                }

                db.ProductImgDetails.Add(newImgDetail);
            }
            if(imgDetails.Count == 2)
            {
                ProductImgDetail newImgDetail = new ProductImgDetail();
                newImgDetail.ProductID = productID;

                if (imgSrc1.Length > 0)
                {
                    imgDetails[0].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc2.Length > 0)
                {
                    imgDetails[1].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc3.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }
                else if (imgSrc4.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }

                if (newImgDetail.ImgDetailSrc == null || newImgDetail.ImgDetailSrc.Length <= 0)
                {
                    return;
                }

                db.ProductImgDetails.Add(newImgDetail);
            }
            if (imgDetails.Count == 3)
            {
                ProductImgDetail newImgDetail = new ProductImgDetail();
                newImgDetail.ProductID = productID;

                if (imgSrc1.Length > 0)
                {
                    imgDetails[0].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc2.Length > 0)
                {
                    imgDetails[1].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc3.Length > 0)
                {
                    imgDetails[2].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc4.Length > 0)
                {
                    newImgDetail.ImgDetailSrc = imgSrc1;
                }

                if (newImgDetail.ImgDetailSrc == null || newImgDetail.ImgDetailSrc.Length <= 0)
                {
                    return;
                }

                db.ProductImgDetails.Add(newImgDetail);
            }

            if (imgDetails.Count == 4)
            {
                if (imgSrc1.Length > 0)
                {
                    imgDetails[0].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc2.Length > 0)
                {
                    imgDetails[1].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc3.Length > 0)
                {
                    imgDetails[2].ImgDetailSrc = imgSrc1;
                }

                if (imgSrc4.Length > 0)
                {
                    imgDetails[3].ImgDetailSrc = imgSrc1;
                }
            }

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
            List<ProductImgDetail> imgDetails = pro.ProductImgDetails.ToList();
            if (imgDetails.Count <= 4)
            {
                updateProductImages(db, imgDetails, pro.ProductID, imgSrc1, imgSrc2, imgSrc3, imgSrc4);
            }

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