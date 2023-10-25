using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class Cart
    {
        public string ProductID { get; set; }
        public string Quantity { get; set; }

        public Cart() { }
        public Cart(string productID, string quantity)
        {
            this.ProductID = productID;
            this.Quantity = quantity;
        }
    }
}