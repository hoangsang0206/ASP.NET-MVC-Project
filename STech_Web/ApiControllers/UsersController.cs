using STech_Web.ApiModels;
using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace STech_Web.ApiControllers
{
    public class UsersController : ApiController
    {
        public CustomerAPI GetCustomer(string customerID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Customer customer = db.Customers.FirstOrDefault(t => t.CustomerID == customerID);
            CustomerAPI customerAPI = new CustomerAPI();
            if(customer != null)
            {
                customerAPI = new CustomerAPI(customer.CustomerID, customer.CustomerName, customer.Phone, customer.Address, customer.Gender, customer.DoB, customer.Email);
            }

            return customerAPI;
        }
    }
}
