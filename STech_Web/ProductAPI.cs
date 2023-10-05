using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web
{
    public class ProductAPI
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImgSrc { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Warranty { get; set; }
        public string BrandID { get; set; }
        public string CateID { get; set; }

        public ProductAPI() { } 

        public ProductAPI(string ProductID, string ProductName, string ImgSrc, decimal Cost, decimal Price, int Warranty, string BrandID, string CateID)
        {
            this.ProductID = ProductID;
            this.ProductName = ProductName;
            this.ImgSrc = ImgSrc;
            this.Cost = Cost;
            this.Price = Price;
            this.Warranty = Warranty;
            this.BrandID = BrandID;
            this.CateID = CateID;
        }
    }
}