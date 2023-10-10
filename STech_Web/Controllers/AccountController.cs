using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.ViewModel;
using STech_Web.Identity;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace STech_Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Register register)
        {
            if(ModelState.IsValid)
            {
                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var passwordHash = Crypto.HashPassword(register.Password);
                var user = new AppUser() { 
                    Email = register.Email,
                    UserName = register.Username, 
                    PasswordHash = passwordHash
                };

                IdentityResult identityResult = userManager.Create(user);
                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Customer");

                    var authenManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenManager.SignIn(new AuthenticationProperties(), userIdentity);
                }

                return Redirect("/");
            }
            else
            {
                return Redirect("/");
            }
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.Find(login.Username, login.Password);

            if (user != null)
            {
                var authenManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenManager.SignIn(new AuthenticationProperties(), userIdentity);

                //return Redirect("/");
                return Json(new { success = true });
            }
            else
            {
                //return Redirect("/");
                return Json(new { success = false });
            }
        }

        public ActionResult Logout()
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            authenManager.SignOut();

            return Redirect("/");
        }
    }
}