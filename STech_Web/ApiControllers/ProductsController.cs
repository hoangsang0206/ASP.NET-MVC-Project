using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using STech_Web.Models;

namespace STech_Web.ApiControllers
{
    public class ProductsController : ApiController
    {
        //Lấy tất cả sản phẩm
        public List<ProductAPI> Get()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.ToList();
            List<ProductAPI> productsAPI = new List<ProductAPI>();

            foreach (Product p in products)
            {
                if (p.Cost == null)
                {
                    p.Cost = 0;
                }
                ProductAPI product = new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID, (int)p.WareHouse.Quantity);
                productsAPI.Add(product);
            }

            return productsAPI;
        }

        //Lấy sản phẩm theo tên (tìm kiếm)
        public List<ProductAPI> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.SearchName(name).ToList();
            List<ProductAPI> productAPIs = new List<ProductAPI>();

            foreach (Product p in products)
            {
                ProductAPI productAPI =
                    new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID, (int)p.WareHouse.Quantity);

                productAPIs.Add(productAPI);
            }

            return productAPIs;

        }

        //Lấy sản phẩm theo danh mục
        public List<ProductAPI> GetByCategory(string cateID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products  = db.Products.Where(t => t.CateID == cateID).ToList();
           
            List<ProductAPI> productsApi = new List<ProductAPI>();

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

        //Lấy sản phẩm theo hãng
        public List<ProductAPI> GetByBrand(string brandID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.Where(t => t.BrandID == brandID).ToList();
            List<ProductAPI> productsApi = new List<ProductAPI>();

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
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.Where(t => t.WareHouse.Quantity <= 0).ToList();
            List<ProductAPI> productsApi = new List<ProductAPI>();


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

    }
}
