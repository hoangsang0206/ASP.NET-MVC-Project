using Microsoft.AspNet.Identity;
using STech_Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace STech_Web.ApiControllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ApiController
    {
        public AppUser GetOne(string username)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindByName(username);

            return user;
        }
    }
}
