using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace STech_Web.ApiContronllers
{
    public class BrandsController : ApiController
    {
        
        public List<Brand> Get()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Brand> brands = db.Brands.ToList();
            
            return brands;
        }
    }
}
