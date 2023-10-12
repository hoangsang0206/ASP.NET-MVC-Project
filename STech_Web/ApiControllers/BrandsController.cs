using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace STech_Web
{
    public class BrandsController : ApiController
    {
        public List<BrandAPI> Get()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Brand> brands = db.Brands.ToList();
            List<BrandAPI> brandAPIs = new List<BrandAPI>();

            foreach (Brand brand in brands)
            {
                BrandAPI brandAPI = new BrandAPI();
                brandAPI.BrandID = brand.BrandID;
                brandAPI.BrandName = brand.BrandName;
                brandAPI.Phone = brand.Phone;
                brandAPI.BrandAddress = brand.BrandAddress;
                brandAPI.BrandImgSrc = brand.BrandImgSrc;

                brandAPIs.Add(brandAPI);
            }

            return brandAPIs;
        }
    }
}
