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
        public List<ProductAPI> GetAll()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.ToList();
            List<ProductAPI> productsAPI = new List<ProductAPI>();

            foreach(Product p in products)
            {
                if(p.Cost == null)
                {
                    p.Cost = 0;
                }
                ProductAPI product = new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price,(int)p.Warranty, p.BrandID, p.CateID);
                productsAPI.Add(product);
            }

            return productsAPI;
        }

        public List<ProductAPI> Get(string cateID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.Where(t => t.CateID == cateID).ToList();
            List<ProductAPI> productsApi = new List<ProductAPI>();

            if (products.Count > 0)
            {
                foreach (Product p in products)
                {
                    if(p.Cost == null)
                    {
                        p.Cost = 0;
                    }

                    ProductAPI product = new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID);

                    productsApi.Add(product);
            }
            }

            return productsApi;
        }
    }
}
