using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using STech_Web.Models;
using System.Web.Hosting;

namespace STech_Web.Models
{
    public class CartTemp
    {
        public List<ProductInCart> cartTemp = new List<ProductInCart>();

        string filePath = HostingEnvironment.MapPath("~/DataFiles/CartTemp.json");

        public CartTemp() 
        {

        }

        //Load cart data from JSON
        public void loadDataFromJSON(string filePath)
        {
            string json = File.ReadAllText(filePath);
            cartTemp = JsonConvert.DeserializeObject<List<ProductInCart>>(json);

            if (cartTemp == null)
            {
                cartTemp = new List<ProductInCart>();
            }
        }

        //--Add product to card when user not logged in
        public void AddToCart(ProductInCart pro)
        {
            loadDataFromJSON(filePath);
            cartTemp.Add(pro);

            string updateJSON = JsonConvert.SerializeObject(cartTemp, Formatting.Indented);
            File.WriteAllText(filePath, updateJSON);
        }

        //--Delete
        public void DeleteFromCart(ProductInCart pro) 
        {
            loadDataFromJSON(filePath);
            cartTemp.Remove(pro);

            string updateJSON = JsonConvert.SerializeObject(cartTemp, Formatting.Indented);
            File.WriteAllText(filePath, updateJSON);
        }
        
        //--Update quantity
        public void updateQuantity(ProductInCart pro, int quantity)
        {
            if (quantity <= 0)
            {
                quantity = 0;
            }
            loadDataFromJSON(filePath);
            cartTemp.FirstOrDefault(t => t.ProductID == pro.ProductID).Quantity = quantity;

            string updateJSON = JsonConvert.SerializeObject(cartTemp, Formatting.Indented);
            File.WriteAllText(filePath, updateJSON);
        }

        //--Count item in cart
        public int countItem()
        {
            return cartTemp.Count();
        }

        //--Get data from cart temp
        public List<ProductInCart> getCartData()
        {
            loadDataFromJSON(filePath);
            return cartTemp;
        }

        //--Add all item to database
        public void addAllToDB(string CustomerID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Cart> cart = new List<Cart>();
            loadDataFromJSON(filePath);

            if(countItem() > 0)
            {
                foreach(ProductInCart product in cartTemp)
                {
                    Cart cartItem = new Cart();
                    cartItem.ProductID = product.ProductID;
                    cartItem.Quantity = product.Quantity;
                    cartItem.CustomerID = CustomerID;

                    cart.Add(cartItem);
                }

                db.Carts.AddRange(cart);
                db.SaveChanges();
            }
        }
    }
}