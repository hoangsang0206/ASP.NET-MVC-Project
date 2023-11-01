using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using Microsoft.AspNet.Identity;
using System.Web.Management;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using STech_Web.Identity;
using System.Globalization;
using System.Text;
using Stripe;
using Stripe.Checkout;
using System.Web.Http.Results;
using System.Net;

namespace STech_Web.Controllers
{
    [UserAuthorization]
    public class OrderController : Controller
    {
        //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

        // GET: Order
        public ActionResult OrderInfo()
        {
            var base64String = Request.Cookies["OrderTemp"]?.Value;
            OrderTemp orderTemp = new OrderTemp();

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);

                if(orderTemp == null)
                {
                    return Redirect("/cart");
                }
            }
            else
            {
                return Redirect("/cart");
            }

            //--------------------
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(User.Identity.GetUserId());

            ViewBag.User = user;
            ViewBag.Order = orderTemp;

            //----------------------
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            string userID = User.Identity.GetUserId();
            List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();
            decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);

            ViewBag.TotalPrice = totalPrice.ToString("##,###", cul);

            return View();
        }

        //Thanh toán
        [HttpPost]
        public ActionResult CheckOut(string paymentMethod)
        {
            //--
            string userID = User.Identity.GetUserId();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();

            //--Lấy thông tin đơn hàng đã lưu tạm vào cookie
            var base64String = Request.Cookies["OrderTemp"]?.Value;
            OrderTemp orderTemp = new OrderTemp();

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);
                if (orderTemp == null)
                {
                    return Redirect("/cart");
                }
            }

            //Tạo khách hàng mới nếu khách hàng này chưa tồn tại
            STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
            if (customer == null)
            {
                addNewCustomer(db, userID);
                db = new DatabaseSTechEntities();
            }

            //Tạo đơn hàng
            int orderCount = db.Orders.ToList().Count;
            string orderID = "DH" + (orderCount + 1).ToString().PadLeft(10, '0');
            decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);

            Order order = new Order();
            order.OrderID = orderID;
            order.OrderDate = DateTime.Now;
            order.Note = orderTemp.Note;
            order.ShipMethod = orderTemp.ShipMethod;
            order.PaymentMethod = paymentMethod;
            order.DeliveryFee = 0;
            order.TotalPrice = totalPrice;
            order.TotalPaymentAmout = totalPrice;
            order.Status = "Chờ thanh toán";

            //Tạo chi tiết đơn hàng
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach(Cart c in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = orderID;
                orderDetail.ProductID = c.ProductID;
                orderDetail.Quantity = c.Quantity;
                
                orderDetails.Add(orderDetail);
            }
            

            //--------------------------------------------------------------------------------
            if (paymentMethod == "COD") //Thanh toán khi nhận hàng
            {

            }
            else if(paymentMethod == "card") //Thanh toán bằng thẻ visa/mastercard
            {
                //Stripe api key
                StripeConfiguration.ApiKey = "sk_test_51O7hGcASuMBoFWl8pQdaMvQaPYFY13MjLln9m2w2oQ41K5JuagkbAJLJmQ8pULQ48ebIgYx9RKCZeAT575F3qoVR00tx24Pnvt";

                // Số tiền cần thanh toán => đổi sang USD (đơn vị là cents - 1 USD  = 100 cents)
                var amount = ((long)totalPrice / 24000) * 100;
                //-------

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Thanh toán đơn hàng " + orderID,
                                },
                                UnitAmount = amount,
                            },
                            Quantity = 1,
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = "http://localhost:49944/cart",
                    CancelUrl = "http://localhost:49944/cart"

                };

                var service = new SessionService();
                var session = service.Create(options);
                //Response.Headers.Add("Location", session.Url);

                return Redirect(session.Url);

                //if(charge.Status == "succeeded")
                //{
                //    order.Status = "Đã thanh toán";
                //    db.Orders.Add(order);
                //    db.OrderDetails.AddRange(orderDetails);
                //    db.SaveChanges();

                //    return Redirect("/order/success");
                //}
                //else
                //{
                //    order.Status = "Thanh toán thất bại";
                //    db.Orders.Add(order);
                //    db.OrderDetails.AddRange(orderDetails);
                //    db.SaveChanges();

                //    return Redirect("/order/failed");
                //}
            }
            else if(paymentMethod == "paypal") //Thanh toán bằng Paypal
            {

            }
            else
            {
                return Redirect("/order/orderinfo");
            }

            return Redirect("/order/failed");
        }

        //--Add new customer ---------------
        public string addNewCustomer(DatabaseSTechEntities db, string userID)
        {

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            STech_Web.Models.Customer customer = new STech_Web.Models.Customer();
            int customersCount = db.Customers.Count();
            string customerID = "KH" + (customersCount + 1).ToString().PadLeft(8, '0');

            customer = new STech_Web.Models.Customer();
            customer.AccountID = userID;
            customer.CustomerID = customerID;
            customer.CustomerName = user.UserFullName;
            customer.Address = user.Address;
            customer.Phone = user.PhoneNumber;
            customer.Email = user.Email;
            customer.DoB = user.DOB;
            customer.Gender = user.Gender;

            db.Customers.Add(customer);
            db.SaveChanges();

            return customerID;

        }

        //Check order infomation 
        [HttpPost]
        public ActionResult CheckOrderInfo(string gender, string customerName, string customerPhone, string address, string shipMethod, string note)
        {
            //Không được để trống thông tin
            if (String.IsNullOrEmpty(gender) || String.IsNullOrEmpty(customerName) || String.IsNullOrEmpty(customerPhone) || String.IsNullOrEmpty(shipMethod))
            {
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
            }

            //Kiểm tra số điện thoại
            if (!(customerPhone.StartsWith("0")) || customerPhone.Length != 10 || !Regex.IsMatch(customerPhone, @"^[0-9]+$"))
            {
                string err = "Số điện thoại không hợp lệ.";
                return Json(new { success = false, error = err });
            }

            //Kiểm tra địa chỉ nhận hàng nếu chọn COD
            if(shipMethod == "COD")
            {
                if(String.IsNullOrEmpty(address))
                {
                    string err = "Vui lòng nhập địa chỉ nhận hàng.";
                    return Json(new { success = false, error = err });
                }
            }

            //Tạo đơn hàng nháp
            OrderTemp orderTemp = new OrderTemp(shipMethod, note);
            var orderTempJson = JsonConvert.SerializeObject(orderTemp);
            var bytesToEncode = Encoding.UTF8.GetBytes(orderTempJson);
            var base64String = Convert.ToBase64String(bytesToEncode);

            //--Save cart item list to cookie
            Response.Cookies["OrderTemp"].Value = base64String;
            //Cookie will expire in 30 days from the date the new product is added
            Response.Cookies["OrderTemp"].Expires = DateTime.Now.AddMinutes(30);

            return Json( new { success = true });
        }
    }
}