using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class ProductInCart
    {
        public string ProductID { get; set; }
        public int Quantity { get; set; }

        public ProductInCart() { }
        public ProductInCart(string productID, int quantity)
        {
            this.ProductID = productID;
            this.Quantity = quantity;
        }
    }
}