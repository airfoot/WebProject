using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Infrastructure;
using WebProject.Domain;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebProject.Models;
using System.IO;
using X.PagedList;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ILogger _logger;
        private WebProjectDbContext _context;
        public AccountController(ILogger logger, WebProjectDbContext webProjectDbContext)
        {
            this._context = webProjectDbContext;
            this._logger = logger;
        }
        [HttpGet]
        public ActionResult GetPersonInformation()
        {
            string userId = this.User.Identity.GetUserId();
            User currentUser = UserManager.FindById(userId);
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.Id = currentUser.Id;
            userViewModel.FullName = currentUser.FullName;
            userViewModel.UserName = currentUser.UserName;
            userViewModel.Email = currentUser.Email;
            userViewModel.Title = currentUser.Title;
            userViewModel.Department = currentUser.Department;
            userViewModel.DirectManagerEmail = currentUser.DirectManagerEmail;
            userViewModel.HireDate = currentUser.HireDate;
            userViewModel.IdentityNumber = currentUser.IdentityNumber;
            userViewModel.ContactAddress = currentUser.ContactAddress;
            userViewModel.ContactPhone = currentUser.PhoneNumber;

            return View(userViewModel);
        }

        [HttpGet]
        public ActionResult ChangePersonInformation()
        {
            string userId = this.User.Identity.GetUserId();
            User currentUser = UserManager.FindById(userId);
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.Id = currentUser.Id;
            userViewModel.FullName = currentUser.FullName;
            userViewModel.UserName = currentUser.UserName;
            userViewModel.Email = currentUser.Email;
            userViewModel.Title = currentUser.Title;
            userViewModel.Department = currentUser.Department;
            userViewModel.DirectManagerEmail = currentUser.DirectManagerEmail;
            userViewModel.HireDate = currentUser.HireDate;
            userViewModel.IdentityNumber = currentUser.IdentityNumber;
            userViewModel.ContactAddress = currentUser.ContactAddress;
            userViewModel.ContactPhone = currentUser.PhoneNumber;

            return View(userViewModel);

        }
        [HttpPost]
        public ActionResult ChangePersonInformation(string id, UserViewModel userViewModel)
        {

            string userId = this.User.Identity.GetUserId();
            User currentUser = UserManager.FindById(userId);
            currentUser.IdentityNumber = userViewModel.IdentityNumber;
            currentUser.ContactAddress = userViewModel.ContactAddress;
            currentUser.PhoneNumber = userViewModel.ContactPhone;
            var changeResult = UserManager.Update(currentUser);

            if (changeResult.Succeeded)
            {
                return RedirectToAction("GetPersonInformation");
            }

            AddErrors(changeResult);
            return View(userViewModel);

        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.HireDate = DateTime.Now;
            return View(userViewModel);
        }
        [HttpPost]
        public ActionResult CreateUser(UserViewModel userViewModel,HttpPostedFileBase userPicture=null)
        {
            string userPictureFilePath = null;
            IdentityResult result = null;
            if (ModelState.IsValid)
            {
                
                if (userPicture != null)
                {
                    //Save uploaded file to path of "~/Images/UserPicture"
                    int splitIndex = userPicture.ContentType.LastIndexOf(@"/");
                    string newFileName = Guid.NewGuid().ToString()+"."  + userPicture.ContentType.Substring(++splitIndex);
                    userPictureFilePath = Path.Combine(Server.MapPath("/Images/UserPicture"), newFileName);
                    if(!Directory.Exists(Server.MapPath("/Images/UserPicture")))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Images/UserPicture"));
                    }
                    userPicture.SaveAs(userPictureFilePath);

                }
                else
                {
                    userPictureFilePath = Path.Combine(Server.MapPath("/Images"), "defaultUser.jpg");
                }
                if (!ValidateEmail.IsValidEmail(userViewModel.Email))
                {
                    ModelState.AddModelError("Email", "邮箱地址无效");
                if (!string.IsNullOrEmpty(userPictureFilePath) && !userPictureFilePath.Contains("defaultUser.jpg"))
                {
                    System.IO.File.Delete(userPictureFilePath);
                }
                    return View(userViewModel);
                }
                if (!ValidateEmail.IsValidEmail(userViewModel.DirectManagerEmail))
                {
                    ModelState.AddModelError("DirectManagerEmail", "邮箱地址无效");
                if (!string.IsNullOrEmpty(userPictureFilePath) && !userPictureFilePath.Contains("defaultUser.jpg"))
                {
                    System.IO.File.Delete(userPictureFilePath);
                }
                return View(userViewModel);
                }
                User user = new User();
                user.FullName = userViewModel.FullName;
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.Title = userViewModel.Title;
                user.Department = userViewModel.Department;
                user.DirectManagerEmail = userViewModel.DirectManagerEmail;
                user.HireDate = userViewModel.HireDate;
                user.IdentityNumber = userViewModel.IdentityNumber;
                user.ContactAddress = userViewModel.ContactAddress;
                user.PhoneNumber = userViewModel.ContactPhone;
                user.UserIconURL = userPictureFilePath;
                if(string.IsNullOrEmpty(userViewModel.Password))
                {
                    result = UserManager.Create(user);
                }
                else
                {
                    result = UserManager.Create(user, userViewModel.Password);
                }
                    
                if (result.Succeeded)
                {
                    return RedirectToAction("GetUsers", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                       
                }
                //Clean before created file if encounter any errors
                if (!string.IsNullOrEmpty(userPictureFilePath) && !userPictureFilePath.Contains("defaultUser.jpg"))
                {
                    System.IO.File.Delete(userPictureFilePath);
                }

            }
            
            return View(userViewModel);
        }

        // GET: Account
        [HttpGet]
        public ActionResult GetUsers(int? page)
        {
            var allUsers = from u in UserManager.Users
                           where u.LockoutEnabled == false
                           orderby u.HireDate descending
                           select new UserViewModel
                           {
                               Id = u.Id,
                               FullName = u.FullName,
                               UserName = u.UserName,
                               Email = u.Email,
                               Title = u.Title,
                               Department = u.Department,
                               DirectManagerEmail = u.DirectManagerEmail,
                               HireDate = u.HireDate,
                               IdentityNumber = u.IdentityNumber,
                               ContactAddress = u.ContactAddress,
                               ContactPhone = u.PhoneNumber
                           };

            var pageNumber = page ?? 1;
            var onePageOfUsers = allUsers.ToPagedList(pageNumber, 15);
            return View(onePageOfUsers);
        }

        /// <summary>
        /// Gets the user picture.
        /// </summary>
        /// <param name="Id">The Id of User object.</param>
        /// <returns></returns>
        [HttpGet]
        public FilePathResult GetUserPicture(string id)
        {
            User user = UserManager.FindById(id);
            if(string.IsNullOrEmpty(user.UserIconURL))
            {
                return File(Path.Combine(Server.MapPath("/Images"), "defaultUser.jpg"),"image/jpeg");
            }
            string contentType =  System.Web.MimeMapping.GetMimeMapping(user.UserIconURL);
            return File(user.UserIconURL, contentType);

        }

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            User user = UserManager.FindById(id);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Title = user.Title,
                Department = user.Department,
                DirectManagerEmail = user.DirectManagerEmail,
                HireDate = user.HireDate,
                IdentityNumber = user.IdentityNumber,
                ContactAddress = user.ContactAddress,
                ContactPhone = user.PhoneNumber,
                Password = ""
            };

            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult EditUser(string id, UserViewModel userViewModel)
        {
            User user = UserManager.FindById(id);
            if(userViewModel.Password!=null && userViewModel.Password.Length>=6)
            {
                try {
                User cUser = UserManager.FindById(id);
                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(userViewModel.Password);
                UserStore<User> store = new UserStore<User>();
                store.SetPasswordHashAsync(cUser, hashedNewPassword);
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("", "重置密码失败");
                    return View(userViewModel);
                }
             
            }
            user.FullName = userViewModel.FullName;
            user.UserName = userViewModel.UserName;
            user.Email = userViewModel.Email;
            user.Title = userViewModel.Title;
            user.Department = userViewModel.Department;
            user.DirectManagerEmail = userViewModel.DirectManagerEmail;
            user.HireDate = userViewModel.HireDate;
            user.IdentityNumber = userViewModel.IdentityNumber;
            user.ContactAddress = userViewModel.ContactAddress;
            user.PhoneNumber = userViewModel.ContactPhone;

            IdentityResult updateResult = UserManager.Update(user);
            if(updateResult.Succeeded)
            {
                return RedirectToAction("GetUsers");
            }

            AddErrors(updateResult);
            return View(userViewModel);


        }


        public ActionResult DeleteUser(string id)
        {
            User user = UserManager.FindById(id);
            var deleteResult = UserManager.SetLockoutEnabled(user.Id,true);
            if(deleteResult.Succeeded)
            {
                return RedirectToAction("GetUsers");
            }
            ViewBag.ErrorMessages = deleteResult.Errors;
            return View("Errors");
            
        }
        [HttpPost]
        public ActionResult UpdateUserPicture(HttpPostedFileBase userPicture,string id)
        {
            string userPictureFilePath = null;
            if(ModelState.IsValid)
            {
                User user = UserManager.FindById(id);
                if (userPicture != null)
                {
                    //Save uploaded file to path of "~/Images/UserPicture"
                    int splitIndex = userPicture.ContentType.LastIndexOf(@"/");
                    string newFileName = Guid.NewGuid().ToString() + "." + userPicture.ContentType.Substring(++splitIndex);
                    userPictureFilePath = Path.Combine(Server.MapPath("/Images/UserPicture"),  newFileName);
                    if (!Directory.Exists(Server.MapPath("/Images/UserPicture")))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Images/UserPicture"));
                    }
                    userPicture.SaveAs(userPictureFilePath);

                    if(!string.IsNullOrEmpty(user.UserIconURL) && !user.UserIconURL.Contains("defaultUser.jpg"))
                    {
                        try
                        {
                            if(System.IO.File.Exists(user.UserIconURL))
                            {
                                System.IO.File.Delete(user.UserIconURL);
                            }
                            
                        }
                        catch(Exception e)
                        {
                           _logger.Error(e.Message, e);
                        }

                        
                    }
                    user.UserIconURL = userPictureFilePath;
                    //Save updated user picture
                   var updateResult= UserManager.Update(user);
                   if(updateResult.Succeeded)
                    { 
                    return Json("");
                    }

                }

            }
            //if error occured, return below Json data according to Fileinput plugin doc.
            string error = "上传失败";
            string[] initialPreview = { };
            string[] initialPreviewConfig = { };
            string[] initialPreviewThumbTags = { };
            bool append = true;
            var back = new
            {
                error = error,
                initialPreview = initialPreview,
                initialPreviewConfig = initialPreviewConfig,
                initialPreviewThumbTags = initialPreviewThumbTags,
                append = append
            };
            return Json(back);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Errors", new string[] { "Access Denied" });
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //User user = UserManager.Find(login.Username, login.Password);
                User user = UserManager.Find(login.Username, login.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    ClaimsIdentity ident = UserManager.CreateIdentity(user,
                        DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    }, ident);

                    LogLogin(user);
                    return RedirectToLocal(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(login);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ResetPassword(Password password)
        {           
            if (ModelState.IsValid)
            {
                 var result = UserManager.ChangePassword(User.Identity.GetUserId(), password.OldPassword, password.NewPassword);
                 if (result.Succeeded)
                 {
                     var returnSuccess = new { isSuccess = true, ModelStateErrors = "",};
                     return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnSuccess), "application/json");
                 }
                 else
                 {
                    AddErrors(result);
                 }
                
            }
            var returnFail = new
             {
                isSuccess = false,
                // ModelState错误信息
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
                  .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
             };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnFail), "application/json");
            
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //private WebProjectUserManager UserManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().GetUserManager<WebProjectUserManager>();
        //    }
        //}
        private WebProjectUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<WebProjectUserManager>();
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void LogLogin(User user)
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            UserLoginLog userLoginLog = new UserLoginLog();
            userLoginLog.Id = Guid.NewGuid().ToString();
            userLoginLog.UserId = user.Id;

            userLoginLog.UserName = user.UserName;
            userLoginLog.RemoteIP = ipAddress;
            userLoginLog.LoginDateTime = DateTime.Now;

            _context.UserLoginLogs.Add(userLoginLog);
            _context.SaveChanges();
        }


    }
}