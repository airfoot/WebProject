using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using WebProject.Domain;


namespace WebProject.Infrastructure
{

    public class WebProjectUserManager : UserManager<User>
    {

        public WebProjectUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static WebProjectUserManager Create(
                IdentityFactoryOptions<WebProjectUserManager> options,
                IOwinContext context)
        {

            WebProjectDbContext dbContext = context.Get<WebProjectDbContext>();
            WebProjectUserManager userManager = new WebProjectUserManager(new UserStore<User>(dbContext));

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
            };
            userManager.UserValidator = new UserValidator<User>(userManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            return userManager;
        }
    }
}