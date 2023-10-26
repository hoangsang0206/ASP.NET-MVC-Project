using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using STech_Web.Filters;

namespace STech_Web.Models
{
    public class Cart
    {
        List<ProductInCart> cart = new List<ProductInCart>();
        public Cart() { }

        //Thêm vào giỏ hàng
        public void AddToCart(ProductInCart pro) 
        { 
            if(pro.ProductID != null && pro.ProductID.Length > 0 && pro.Quantity > 0)
            {
                cart.Add(pro);
            }
            
        }

        //Thêm vào giỏ hàng khi user đã đăng nhập
        //dữ liệu giỏ hàng sẽ thêm vào database
        [UserAuthorization]
        public void AddtoCartWhenLoggedIn(ProductInCart pro) 
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            if (pro.ProductID != null && pro.ProductID.Length > 0 && pro.Quantity > 0)
            {
                cart.Add(pro);
            }
        }
    }
}