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
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == id);
                if(product == null)
                {
                    return Redirect("/admin/products");
                }

                List<Category> categories = db.Categories.ToList();
                List<Brand> brands = db.Brands.ToList();
                ProductImgDetail imgDetail = product.ProductImgDetail;

                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.ImgDetail = imgDetail;
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
        public JsonResult AddProduct(Product product, int quantity, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
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
                db.Products.Add(product);

                //---
                WareHouse wh = new WareHouse();
                wh.ProductID = product.ProductID;
                wh.Quantity = quantity;
                db.WareHouses.Add(wh);

                //---
                ProductImgDetail imgDetail = new ProductImgDetail();
                updateProductImages(db, imgDetail, product.ProductID, imgSrc1, imgSrc2, imgSrc3, imgSrc4);
                //---
                db.SaveChanges();
                //---
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

        //Update product
        [HttpPost]
        public JsonResult UpdateProduct(Product product, int quantity, string imgSrc1, string imgSrc2, string imgSrc3, string imgSrc4)
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
                pro.Cost = product.Cost;
                pro.Price = product.Price;
                pro.CateID = product.CateID;
                pro.BrandID = product.BrandID;
                pro.Warranty = product.Warranty;

                //--------------
                WareHouse wh = pro.WareHouse;
                wh.Quantity = quantity;

                //--------------
                ProductImgDetail imgDetail = pro.ProductImgDetail;
                updateProductImages(db, imgDetail, pro.ProductID, imgSrc1, imgSrc2, imgSrc3, imgSrc4);

                //----------
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }


        //Delete product
        [HttpPost]
        public JsonResult DeleteProduct(string productID)
        {
            try
            {

                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                if (product == null)
                {
                    return Json(new { success = false, error = "Sản phẩm này không tồn tại." });
                }

                //--------------------
                WareHouse wh = product.WareHouse;
                ProductImgDetail imgDetail = product.ProductImgDetail;
                ProductOutStanding proOSD = product.ProductOutStandings.FirstOrDefault(t => t.ProductID == product.ProductID);
                Sale productSale = product.Sales.FirstOrDefault(t => t.ProductID == product.ProductID);
                List<OrderDetail> orderDetailList = db.OrderDetails.Where(t => t.ProductID == productID).ToList();
                List<Cart> cartList = db.Carts.Where(t => t.ProductID == productID).ToList();

                if (orderDetailList.Count > 0)
                {
                    return Json(new { success = false, error = "Không thể xóa sản phẩm này." });
                }

                //-----------------------
                if (wh != null)
                {
                    db.WareHouses.Remove(wh);
                }
                if (imgDetail != null)
                {
                    db.ProductImgDetails.Remove(imgDetail);
                }
                if (proOSD != null)
                {
                    db.ProductOutStandings.Remove(proOSD);
                }
                if (productSale != null)
                {
                    db.Sales.Remove(productSale);
                }
                if (cartList.Count > 0)
                {
                    db.Carts.RemoveRange(cartList);
                }

                //----------------------

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //Delete all product in category
        public JsonResult DeleteAllInCategory(string cateID)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Category category = db.Categories.FirstOrDefault(t => t.CateID == cateID);

                //Kiểm tra danh mục có tồn tại không
                if (category == null)
                {
                    return Json(new { success = false, error = "Danh mục " + category.CateName + " không tồn tại." });
                }

                //Kiểm tra có sản phẩm nào trong danh mục không
                if (category.Products.Count <= 0)
                {
                    return Json(new { success = false, error = "Không có sản phẩm nào thuộc danh mục " + category.CateName + "." });
                }

                //-----------------
                List<Product> products = category.Products.ToList();
                List<WareHouse> wareHouses = new List<WareHouse>();
                List<ProductImgDetail> productImgDetails = new List<ProductImgDetail>();
                List<ProductOutStanding> productOSDs = new List<ProductOutStanding>();
                List<Sale> productSale = new List<Sale>();

                //------------------
                foreach (Product product in products)
                {
                    List<OrderDetail> orderDetailList = db.OrderDetails.Where(t => t.ProductID == product.ProductID).ToList();
                    List<Cart> cartList = db.Carts.Where(t => t.ProductID == product.ProductID).ToList();

                    if (orderDetailList.Count > 0)
                    {
                        continue;
                    }

                    WareHouse wh = product.WareHouse;
                    if (wh != null)
                    {
                        wareHouses.Add(wh);
                    }

                    ProductImgDetail imgDetail = product.ProductImgDetail;
                    if (imgDetail != null)
                    {
                        productImgDetails.Add(imgDetail);
                    }

                    ProductOutStanding proOSD = product.ProductOutStandings.FirstOrDefault(t => t.ProductID == product.ProductID);
                    if (proOSD != null)
                    {
                        productOSDs.Add(proOSD);
                    }

                    Sale proSale = product.Sales.FirstOrDefault(t => t.ProductID == product.ProductID);
                    if (proSale != null)
                    {
                        productSale.Add(proSale);
                    }

                    db.Carts.RemoveRange(cartList);

                }


                //---------------------------
                if (wareHouses.Count > 0)
                {
                    db.WareHouses.RemoveRange(wareHouses);
                }

                if (productImgDetails.Count > 0)
                {
                    db.ProductImgDetails.RemoveRange(productImgDetails);
                }

                if (productOSDs.Count > 0)
                {
                    db.ProductOutStandings.RemoveRange(productOSDs);
                }

                if (productSale.Count > 0)
                {
                    db.Sales.RemoveRange(productSale);
                }

                db.Products.RemoveRange(products);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

    }
}