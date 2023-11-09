using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.DynamicData;
using System.Web.Http;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.ApiControllers
{
    [AdminAuthorization]
    public class OrdersController : ApiController
    {
        public List<OrderAPI> GetAll()
        {

            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.ToList();

            if (orders.Count > 0)
            {
                foreach (Order order in orders)
                {
                    ordersApi.Add(new OrderAPI(order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod));
                }
            }
            return ordersApi;
        }
        public List<OrderAPI> GetOrders(string searchType, string searchValue)
        {
            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = new List<Order>();

            try
            {
                switch (searchType)
                {
                    case "order-id":
                        orders = db.Orders.Where(t => t.OrderID == searchValue).ToList();
                        break;
                    case "customer-name":
                        orders = db.Orders.Where(t => t.Customer.CustomerName.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                    case "customer-phone":
                        orders = db.Orders.Where(t => t.Customer.Phone == searchValue).ToList();
                        break;
                    case "order-date":
                        DateTime date = DateTime.Parse(searchValue);
                        orders = db.Orders.Where(t => t.OrderDate.Value.Day == date.Day && t.OrderDate.Value.Month == date.Month && t.OrderDate.Value.Year == date.Year).ToList();
                        break;
                    case "order-month":
                        DateTime date1 = DateTime.Parse(searchValue);
                        orders = db.Orders.Where(t => t.OrderDate.Value.Month == date1.Month && t.OrderDate.Value.Year == date1.Year).ToList();
                        break;
                }

                if (orders.Count > 0)
                {
                    foreach (Order order in orders)
                    {
                        ordersApi.Add(new OrderAPI(order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod));
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ordersApi;
        }
    }
}
