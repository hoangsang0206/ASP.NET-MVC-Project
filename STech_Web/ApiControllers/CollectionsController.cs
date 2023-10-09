﻿using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace STech_Web.ApiControllers
{
    public class CollectionsController : ApiController
    {
        //API get products by name
        public List<ProductAPI> Get(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return null;
            }    

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.SearchName(id).ToList();
            List<ProductAPI> productAPIs = new List<ProductAPI>();
            
            foreach (Product p in products)
            {
                if(p.Cost == null)
                {
                    p.Cost = 0;
                }

                ProductAPI productAPI =
                    new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID);

                productAPIs.Add(productAPI);
            }

            return productAPIs;

        }
    }
}