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
            ProductImgDetail imgDetail = product.ProductImgDetail;

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

        //--Add product to backup table
        public void addProductToBackup(DatabaseSTechEntities db ,Product product)
        {
            if(product == null)
            {
                return;
            }

            if(db.ProductsBackups.Find(product.ProductID) != null)
            {
                return;
            }

            ProductsBackup productBAK = new ProductsBackup();
            productBAK.ProductID = product.ProductID;
            productBAK.ProductName = product.ProductName;
            productBAK.Cost = product.Cost;
            productBAK.Price = product.Price;
            productBAK.ImgSrc = product.ImgSrc;
            productBAK.Warranty = product.Warranty;
            productBAK.CateID= product.CateID;
            productBAK.BrandID = product.BrandID;
            
            db.ProductsBackups.Add(productBAK);
        }

        //--Delete product from backup table
        public void deleteProductFromBackup(DatabaseSTechEntities db, Product product)
        {
            if(product == null)
            {
                return;
            }

            ProductsBackup proBAK = db.ProductsBackups.Find(product.ProductID);
            if(proBAK == null)
            {
                return;
            }

            db.ProductsBackups.Remove(proBAK);
        }

        //Delete product
        [HttpPost]
        public ActionResult DeleteProduct(string productID)
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

            //-Lưu sản phẩm vào bảng backup trước khi xóa-----------------------
            //addProductToBackup(db, product);
            //----------------------

            db.Products.Remove(product);
            db.SaveChanges();
            return Json(new { success = true });
        }

        //Delete all product in category
        public ActionResult DeleteAllInCategory(string cateID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Category category = db.Categories.FirstOrDefault(t => t.CateID == cateID);

            //Kiểm tra danh mục có tồn tại không
            if (category == null)
            {
                return Json(new { success = false, error = "Danh mục " + category.CateName +" không tồn tại." });
            }

            //Kiểm tra có sản phẩm nào trong danh mục không
            if(category.Products.Count <= 0)
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
                WareHouse wh = product.WareHouse;
                if(wh != null)
                {
                    wareHouses.Add(wh);
                }

                ProductImgDetail imgDetail = product.ProductImgDetail;
                if (imgDetail != null)
                {
                    productImgDetails.Add(imgDetail);
                }

                ProductOutStanding proOSD = product.ProductOutStandings.FirstOrDefault(t => t.ProductID == product.ProductID);
                if(proOSD != null)
                { 
                    productOSDs.Add(proOSD);
                }

                Sale proSale = product.Sales.FirstOrDefault(t => t.ProductID == product.ProductID);
                if(proSale != null)
                {
                    productSale.Add(proSale);
                }

                //--Lưu sản phẩm vào bảng backup
                addProductToBackup(db, product);
            }

            //--Tạo bảng để lưu dữ liệu cho việc khôi phục

            //---------------------------
            if(wareHouses.Count > 0)
            {
                db.WareHouses.RemoveRange(wareHouses);
            }

            if(productImgDetails.Count > 0)
            {
                db.ProductImgDetails.RemoveRange(productImgDetails);
            }

            if(productOSDs.Count > 0)
            {
                db.ProductOutStandings.RemoveRange(productOSDs);
            }

            if(productSale.Count > 0)
            {
                db.Sales.RemoveRange(productSale);
            }

            db.Products.RemoveRange(products);
            db.SaveChanges();

            return Json(new { success = true });
        }

        //--Function backup one product data -------------
        public bool backupOneProduct(DatabaseSTechEntities db, ProductsBackup proBAK)
        {
            if(proBAK == null)
            {
                return false;
            }

            Product product = new Product();
            product.ProductID = proBAK.ProductID;
            product.ProductName = proBAK.ProductName;
            product.Cost = proBAK.Cost;
            product.Price = proBAK.Price;
            product.ImgSrc = proBAK.ImgSrc;
            product.BrandID = proBAK.BrandID;
            product.CateID = proBAK.CateID;
            product.Warranty = proBAK.Warranty;

            db.Products.Add(product);
            return true;
        }

        //--Backup one product data ----------------------
        public ActionResult BackupOneProductData(ProductsBackup proBAK)
        {
            if(proBAK == null)
            {
                return Json(new { success = false, error = "Sản phẩm này đã bị xóa khỏi cơ sở dữ liệu." });
            }

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            bool result = backupOneProduct(db, proBAK);

            if(result == false)
            {
                return Json(new { success = false, error = "Không thể khôi phục sản phẩm này." });
            }

            return Json(new { success = true });
        }

        //--Backup all product data ----------------------
        public ActionResult BackupAllProductData()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<ProductsBackup> productBAKs = db.ProductsBackups.ToList();

            if(productBAKs.Count <= 0)
            {
                return Json(new { success = false, error = "Không có sản phẩm nào để khôi phục." });
            }

            foreach(ProductsBackup productBAK in productBAKs)
            {
                backupOneProduct(db, productBAK);
            }

            return Json(new { success = true });
        }
    }
}