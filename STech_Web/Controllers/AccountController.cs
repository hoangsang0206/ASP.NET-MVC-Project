using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.ViewModel;
using STech_Web.Identity;
using STech_Web.Filters;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.Management;

namespace STech_Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [UserAuthorization]
        public ActionResult Index()
        {
            ViewBag.ActiveBotNav = "account";
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterVM register)
        {
            if(ModelState.IsValid)
            {
                //Các thông tin không được bỏ trống
                if (register.Username == null ||
                    register.Password == null ||
                    register.ConfirmPassword == null ||
                    register.Email == null)
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var passwordHash = Crypto.HashPassword(register.Password);
                var user = new AppUser() { 
                    Email = register.Email,
                    UserName = register.Username, 
                    PasswordHash = passwordHash
                };

                var existingUser = userManager.FindByName(register.Username);
                var existingUserEmail = userManager.FindByEmail(register.Email);
                bool containsSpace = Regex.IsMatch(register.Username, @"\s");
                bool containsSpecialCharacter = Regex.IsMatch(register.Username, @"[^a-zA-Z0-9_]");

                //Tên đăng nhập không quá 15 ký tự
                if (register.Username.Length > 15)
                {
                    ModelState.AddModelError("", "Tên đăng tối đa 15 ký tự.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                //Tên đăng nhập không chứa khoảng trắng và ký tự đặc biệt
                if (containsSpace || containsSpecialCharacter)
                {
                    ModelState.AddModelError("", "Tên đăng nhập không chứa ký tự đặc biệt trừ '_'.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                //Xác nhận mật khẩu phải trùng với mật khẩu
                if(register.Password != register.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Xác nhận mật khẩu không đúng.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                if(existingUser != null || existingUserEmail != null)
                {
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Error User", "Tài khoản nãy đã tồn tại.");
                    }
                    if(existingUserEmail != null)
                    {
                        ModelState.AddModelError("Email Error", "Email này đã tồn tại.");
                    }

                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                IdentityResult identityResult = userManager.Create(user);

                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Customer");

                    var authenManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenManager.SignIn(new AuthenticationProperties(), userIdentity);

                }

                //return Redirect("/");
                return Json(new { success = true, redirectUrl = "/" });  
            }
            else
            {
                ModelState.AddModelError("", "Dữ liệu không hợp lệ.");
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //return Redirect("/");
                return Json(new { success = false, errors = error });
            }
        }

        [HttpPost]
        public ActionResult Login(LoginVM login)
        {
            //Tên đăng nhập và mật khẩu không để trống
            if (login.Username == null || login.Password == null)
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //return Redirect("/");
                return Json(new { success = false, errors = error });
            }

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.Find(login.Username, login.Password);

            if (user != null)
            {
                var authenManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenManager.SignIn(new AuthenticationProperties(), userIdentity);

                if(userManager.IsInRole(user.Id, "admin"))
                {
                    return Json(new { success = true, redirectUrl = "/admin/dashboard" });
                }

                //return Redirect("/");
                return Json(new { success = true, redirectUrl = "/account" });
            }
            else
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //return Redirect("/");
                return Json(new { success = false, errors = error });
            }
        }

        public ActionResult Logout()
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            authenManager.SignOut();

            return Redirect("/");
        }

        [HttpPost]
        public ActionResult Update(UpdateUserVM update, string userID)
        {

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            if (user != null)
            {
                user.UserFullName = update.UserFullName;
                user.Gender = update.Gender;
                user.PhoneNumber = update.PhoneNumber;
                user.Email = update.Email;
                user.DOB = update.DOB;
                user.Address = update.Address;

                //Kiểm tra ngày sinh phải nhỏ hơn ngày hiện tại
                if(update.DOB > DateTime.Now)
                {
                    ModelState.AddModelError("", "Ngày sinh phải nhỏ hơn ngày hiện tại .");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                //Kiểm tra ngày sinh phải lớn hơn hoặc bằng 01/01/1930
                DateTime oldDate = DateTime.Parse("1930/01/01");
                if (update.DOB <= oldDate)
                {
                    ModelState.AddModelError("", "Ngày sinh phải lớn hơn 01/01/1930 .");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                //Kiểm tra xem email có tồn tại trong bảng user chưa
                var allUsers = userManager.Users.ToList();
                if(allUsers.Any(t => t.Id != userID && t.Email == update.Email))
                {
                    ModelState.AddModelError("", "Email này đã tồn tại.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                //Kiểm tra số điện thoại
                if(!(update.PhoneNumber.StartsWith("0")) || update.PhoneNumber.Length != 10)
                {
                    ModelState.AddModelError("", "Số điện thoại không hợp lệ.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }

                var updateCheck = userManager.Update(user);
                if(updateCheck.Succeeded)
                {
                    return Json(new { success = true });
                }    
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật.");
                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return Redirect("/");
                    return Json(new { success = false, errors = error });
                }
            }
            else
            {
                ModelState.AddModelError("", "Không thể cập nhật.");
                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //return Redirect("/");
                return Json(new { success = false, errors = error });
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword, string userID)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            if(user != null)
            {

                if (oldPassword.Length <= 0 || newPassword.Length <= 0 || confirmNewPassword.Length <= 0)
                {
                    return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
                }

                if(Crypto.VerifyHashedPassword(user.PasswordHash, oldPassword) == false)
                {
                    return Json(new { success = false, error = "Sai mật khẩu." });
                }

                if (newPassword != confirmNewPassword)
                {
                    return Json(new { success = false, error = "Xác nhận mật khẩu không đúng." });
                }

                user.PasswordHash = Crypto.HashPassword(newPassword);
                var updateCheck = userManager.Update(user);
                if (updateCheck.Succeeded)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, error = "Không thể đổi mật khẩu" });
        }
    }
}