using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class CategoryAPI
    {
        public string CateID { get; set; }
        public string CateName { get; set; }

        public CategoryAPI(string CateID, string CateName) 
        {
            this.CateID = CateID;
            this.CateName = CateName;
        }
    }
}