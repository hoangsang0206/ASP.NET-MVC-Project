using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace STech_Web.ApiControllers
{
    public class TestAPIController : ApiController
    {
        public string Post(string id)
        {
            return id;
        }

        public string Get(string id)
        {
            return id;
        }
    }
}
