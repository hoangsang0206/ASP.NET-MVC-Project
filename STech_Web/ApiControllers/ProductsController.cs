using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using STech_Web.Models;
using STech_Web.ApiModels;

namespace STech_Web.ApiControllers
{
    public class ProductsController : ApiController
    {
        //Lấy tất cả sản phẩm
        public List<ProductAPI> Get()
        {
            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }
        }

        //Lấy sản phẩm theo tên (tìm kiếm)
        public List<ProductAPI> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.SearchName(name).Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }
        }

        public List<ProductAPI> GetByNameAndInstock(string nameIS)
        {
            if (string.IsNullOrWhiteSpace(nameIS))
            {
                return null;
            }

            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.SearchName(nameIS).Where(t => t.WareHouse.Quantity > 0).Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }

        }

        //Lấy sản phẩm theo danh mục
        public List<ProductAPI> GetByCategory(string cateID)
        {
            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.Where(t => t.CateID == cateID).Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }
        }

        //Lấy sản phẩm theo hãng
        public List<ProductAPI> GetByBrand(string brandID)
        {
            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.Where(t => t.BrandID == brandID).Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }
        }
        
        //Lấy sản phẩm theo danh mục và hãng
        public List<ProductAPI> GetByCategoryAndBrand(string CateID, string BrandID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = new List<Product>();
            List<ProductAPI> productsApi = new List<ProductAPI>();

            if (BrandID == null)
            {
                products = db.Products.Where(t => t.CateID == CateID).ToList();
            }
            else if(CateID == null)
            {
                products = db.Products.Where(t => t.BrandID == BrandID).ToList();
            }
            else
            {
                products = db.Products.Where(t => t.CateID == CateID && t.BrandID == BrandID).ToList();
            }

            if (products.Count > 0)
            {
                foreach (Product p in products)
                {
                    ProductAPI product = new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID, (int)p.WareHouse.Quantity);

                    productsApi.Add(product);
                }
            }

            return productsApi;
        }


        //Lấy sản phẩm đã hết hàng
        public List<ProductAPI> GetOutOfStock(string type)
        {
            using (DatabaseSTechEntities db = new DatabaseSTechEntities())
            {
                List<ProductAPI> products = db.Products.Where(t => t.WareHouse.Quantity <= 0).Select(p => new ProductAPI
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ImgSrc = p.ImgSrc,
                    Cost = p.Cost ?? 0,
                    Price = p.Price != null ? (decimal)p.Price : 0,
                    Warranty = p.Warranty != null ? (int)p.Warranty : 0,
                    BrandID = p.BrandID,
                    CateID = p.CateID,
                    Quantity = p.WareHouse != null ? (int)p.WareHouse.Quantity : 0
                }).ToList();

                return products;
            }
        }

        //Lấy sản phẩm theo mã sản phẩm (sản phẩm còn hàng - tạo đơn hàng)
        public ProductAPI GetByID(string productID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.FirstOrDefault(t => t.ProductID == productID && t.WareHouse.Quantity > 0);
            ProductAPI productApi = new ProductAPI();


            if (product != null)
            {
                productApi = new ProductAPI(product.ProductID, product.ProductName, product.ImgSrc, (decimal)product.Cost, (decimal)product.Price, (int)product.Warranty, product.BrandID, product.CateID, (int)product.WareHouse.Quantity);
            }

            return productApi;
        }

        [Authorize(Roles = "Admin")]
        public bool Delete(string productID)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                if (product == null)
                {
                    return false;
                }
                WareHouse wh = product.WareHouse;
                List<SaleDetail> productSale = product.SaleDetails.ToList();
                List<ProductGift> proGifts = db.ProductGifts.Where(t => t.ProductID == productID).ToList();
                List<OrderDetail> orderDetailList = product.OrderDetails.ToList();
                List<Cart> cartList = product.Carts.ToList();
                List<ProductSpecification> specs = product.ProductSpecifications.ToList();
                List<ProductContent> contents = product.ProductContents.ToList();

                if (orderDetailList.Count > 0)
                {
                    return false;
                }

                //-----------------------
                if (wh != null)
                {
                    db.WareHouses.Remove(wh);
                }
                if(proGifts.Count > 0)
                {
                    db.ProductGifts.RemoveRange(proGifts);
                }
                if (productSale.Count > 0)
                {
                    db.SaleDetails.RemoveRange(productSale);
                }
                if (cartList.Count > 0)
                {
                    db.Carts.RemoveRange(cartList);
                }
                if(specs.Count > 0)
                {
                    db.ProductSpecifications.RemoveRange(specs);
                }
                if (contents.Count > 0)
                {
                    db.ProductContents.RemoveRange(contents);
                }

                //----------------------

                db.Products.Remove(product);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }

    }
}
