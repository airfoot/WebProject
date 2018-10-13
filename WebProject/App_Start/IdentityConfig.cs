using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using WebProject.Infrastructure;

namespace WebProject.App_Start
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {

            app.CreatePerOwinContext<WebProjectDbContext>(WebProjectDbContext.Create);
            app.CreatePerOwinContext<WebProjectUserManager>(WebProjectUserManager.Create);
            app.CreatePerOwinContext<WebProjectRoleManager>(WebProjectRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}