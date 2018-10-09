using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Principal;
using WebProject.Domain;

namespace WebProject.Infrastructure
{
    public static class WebProjectIdentityExtensions
    {
        public static string GetUserFullName(this IIdentity identity)
        {
            WebProjectUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<WebProjectUserManager>();
            User user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            if(user != null)
            {
                return user.FullName;
            }
            return "";
        }
        public static string GetUserEmail(this IIdentity identity)
        {
            WebProjectUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<WebProjectUserManager>();
            User user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            if (user != null)
            {
                return user.Email;
            }
            return "";
         
        }
        public static string GetUserManagerEmail(this IIdentity identity)
        {
            WebProjectUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<WebProjectUserManager>();
            User user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            if (user != null)
            {
                return user.DirectManagerEmail;
            }
            return "";
        }
    }
}