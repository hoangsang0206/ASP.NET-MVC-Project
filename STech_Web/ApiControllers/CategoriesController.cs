using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.DynamicData;
using System.Web.Http;
using STech_Web.Models;

namespace STech_Web.ApiControllers
{
    public class CategoriesController : ApiController
    {
        public List<CategoryAPI> Get()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();
            List<CategoryAPI> categoriesAPI = new List<CategoryAPI>();

            foreach (Category cate in categories)
            {
                CategoryAPI category = new CategoryAPI(cate.CateID, cate.CateName);

                categoriesAPI.Add(category);
            }

            return categoriesAPI;
        }
    }
}
