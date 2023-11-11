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
using System.IO;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.Owin.Security.Provider;

namespace STech_Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [UserAuthorization]
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.Where(t => t.Customer.AccountID == userID).ToList();

            ViewBag.Orders = orders;
            ViewBag.ActiveBotNav = "account";
            return View(user);
        }

        [HttpPost]
        public JsonResult Register(RegisterVM register)
        {
            if(ModelState.IsValid)
            {
                //Các thông tin không được bỏ trống
                if (register.Username == null ||
                    register.Password == null ||
                    register.ConfirmPassword == null ||
                    register.Email == null)
                {
                    string err = "Vui lòng nhập đầy đủ thông tin.";
                    return Json(new { success = false, error = err });
                }

                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var passwordHash = Crypto.HashPassword(register.Password);
                var user = new AppUser() { 
                    Email = register.Email,
                    UserName = register.Username, 
                    PasswordHash = passwordHash,
                    DateCreate = DateTime.Now
                };

                var existingUser = userManager.FindByName(register.Username);
                var existingUserEmail = userManager.FindByEmail(register.Email);
                bool containsSpace = Regex.IsMatch(register.Username, @"\s");
                bool containsSpecialCharacter = Regex.IsMatch(register.Username, @"[^a-zA-Z0-9_]");

                //Tên đăng nhập không quá 15 ký tự
                if (register.Username.Length > 15)
                {
                    string err = "Tên đăng tối đa 15 ký tự.";
                    return Json(new { success = false, error = err });
                }

                //Tên đăng nhập không chứa khoảng trắng và ký tự đặc biệt
                if (containsSpace || containsSpecialCharacter)
                {
                    string err = "Tên đăng nhập không chứa ký tự đặc biệt trừ '_'.";
                    return Json(new { success = false, error = err });
                }

                //Xác nhận mật khẩu phải trùng với mật khẩu
                if(register.Password != register.ConfirmPassword)
                {
                    string err = "Xác nhận mật khẩu không đúng.";
                    return Json(new { success = false, error = err });
                }

                //Kiểm tra xem user đã tồn tại chưa
                if (existingUser != null)
                {
                    string err = "Tài khoản nãy đã tồn tại.";
                    return Json(new { success = false, error = err });
                }

                //Kiểm tra xem email đã tồn tại chưa
                if(existingUserEmail != null)
                {
                    string err = "Email này đã tồn tại.";
                    return Json(new { success = false, error = err });
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
                string err = "Dữ liệu không hợp lệ.";
                return Json(new { success = false, error = err });
            }
        }

        [HttpPost]
        public JsonResult Login(LoginVM login)
        {
            //Tên đăng nhập và mật khẩu không để trống
            if (login.Username == null || login.Password == null)
            {
                string err = "Vui lòng nhập đầy đủ thông tin.";
                return Json(new { success = false, error = err });
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

                return Json(new { success = true, redirectUrl = "" });
            }
            else
            {
                string err = "Sai tên đăng nhập hoặc mật khẩu.";
                return Json(new { success = false, error = err });
            }
        }

        public ActionResult Logout()
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            authenManager.SignOut();

            return Redirect("/");
        }

        //Update account info
        [HttpPost, UserAuthorization]
        public JsonResult Update(UpdateUserVM update)
        {

            if (User.Identity.IsAuthenticated)
            {
                string userID = User.Identity.GetUserId();

                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var user = userManager.FindById(userID);

                if (user != null)
                {
                    //Kiểm tra ngày sinh phải nhỏ hơn ngày hiện tại
                    if (update.DOB > DateTime.Now)
                    {
                        string err = "Ngày sinh phải nhỏ hơn ngày hiện tại.";
                        return Json(new { success = false, error = err });
                    }

                    //Kiểm tra ngày sinh phải lớn hơn hoặc bằng 01/01/1930
                    DateTime oldDate = DateTime.Parse("1930/01/01");
                    if (update.DOB <= oldDate)
                    {
                        string err = "Ngày sinh phải lớn hơn 01/01/1930.";
                        return Json(new { success = false, error = err });
                    }

                    //Kiểm tra xem email có tồn tại trong bảng user chưa
                    var allUsers = userManager.Users.ToList();
                    if (allUsers.Any(t => t.Id != userID && t.Email == update.Email))
                    {
                        string err = "Email này đã tồn tại.";
                        return Json(new { success = false, error = err });
                    }

                    //Kiểm tra số điện thoại
                    if (update.PhoneNumber == null || !(update.PhoneNumber.StartsWith("0")) || update.PhoneNumber.Length != 10 || !Regex.IsMatch(update.PhoneNumber, @"^[0-9]+$"))
                    {
                        string err = "Số điện thoại không hợp lệ.";
                        return Json(new { success = false, error = err });
                    }

                    //------------------------
                    if (user.DOB != null && update.DOB == null)
                    {
                        update.DOB = user.DOB;
                    }

                    if(user.Email.Length > 0 && update.Email == null)
                    {
                        update.Email = user.Email;
                    }

                    //-----------------------
                    user.UserFullName = update.UserFullName;
                    user.Gender = update.Gender;
                    user.PhoneNumber = update.PhoneNumber;
                    user.Email = update.Email;
                    user.DOB = update.DOB;
                    user.Address = update.Address;

                    var updateCheck = userManager.Update(user);
                    if (updateCheck.Succeeded)
                    {
                        return Json(new { success = true });
                    }
                }
            }

            return Json(new { success = false, error = "Không thể cập nhật." });
        }
        
        //Đổi mật khẩu
        [HttpPost, UserAuthorization]
        public JsonResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            if(User.Identity.IsAuthenticated)
            {
                string userID = User.Identity.GetUserId();

                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var user = userManager.FindById(userID);

                if (user != null)
                {

                    if (oldPassword.Length <= 0 || newPassword.Length <= 0 || confirmNewPassword.Length <= 0)
                    {
                        return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
                    }

                    if (Crypto.VerifyHashedPassword(user.PasswordHash, oldPassword) == false)
                    {
                        return Json(new { success = false, error = "Mật khẩu cũ không đúng." });
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
            }
            
            return Json(new { success = false, error = "Không thể đổi mật khẩu" });
        }

        //Upload hình ảnh
        [HttpPost, UserAuthorization]
        public JsonResult UploadImage()
        {
            try
            {
                HttpPostedFileBase imageFile = null;
                if(HttpContext.Request.Files.Count > 0)
                {
                    imageFile = HttpContext.Request.Files[0];
                }

                if (imageFile == null || imageFile.ContentLength <= 0)
                {
                    return Json(new { success = false, error = "Hình ảnh không được để trống." });
                }

                if (imageFile.ContentLength > 5120000)
                {
                    return Json(new { success = false, error = "Kích thước hình ảnh không lớn hơn 5MB." });
                }

                var allowExtensions = new[] { ".jpg", ".png", ".jpeg", ".webp" };
                var fileEx = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowExtensions.Contains(fileEx))
                {
                    return Json(new { success = false, error = "Vui lòng tải lên hình ảnh dạng .jpg, .jpeg, .png, .webp." });
                }

                string userID = User.Identity.GetUserId();
                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var user = userManager.FindById(userID);

                var fileName = user.UserName + '-' + randomString(30);
                string imgSrc = null;
                
                //Upload hình ảnh lên Google Cloud Storage
                string bucketName = "stech-product-images";
                string keyPath = Server.MapPath("/GoogleCloud/magnetic-music-400416-1df1c9c33391.json");

                var credential = GoogleCredential.FromFile(keyPath);
                var storage = StorageClient.Create(credential);

                var objectName = Path.Combine("user-images/", fileName + fileEx);
                storage.UploadObject(bucketName, objectName, "image/" + fileEx.Replace(".", ""), imageFile.InputStream);

                if(storage.GetObject(bucketName, objectName) != null)
                {
                    //Xóa hình ảnh khỏi Google Cloud Storage nếu user đã upload ảnh trước đó
                    if (!string.IsNullOrEmpty(user.ImgSrc))
                    {
                        try
                        {
                            Uri uri = new Uri(user.ImgSrc);
                            string oldFileName = uri.Segments[uri.Segments.Length - 1];
                            storage.DeleteObject(bucketName, "user-images/" + oldFileName);
                        }
                        catch (Google.GoogleApiException ex) { 
                            //Xóa đường dẫn ảnh trong cơ sở dữ liệu nếu không tìm thấy ảnh này trên Cloud
                            if(ex.Error.Code == 404)
                            {
                                user.ImgSrc = null;
                            }
                        }
                            
                    }

                    //Gán đường dẫn hình ảnh đó cho user
                    imgSrc = "https://storage.googleapis.com/stech-product-images/user-images/" + fileName + fileEx;
                    user.ImgSrc = imgSrc;

                    userManager.Update(user);
                }

                return Json(new { success = true, src = imgSrc });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Tải lên thất bại." });
            }
        }

        //Tạo ngẫu nhiên chuỗi
        private string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            string uniqueFileName = new string(randomChars);

            return uniqueFileName;
        }
    }
}