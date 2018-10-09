using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebProject.Domain;

namespace WebProject.Infrastructure
{

    public class WebProjectRoleManager : RoleManager<Role>, IDisposable
    {

        public WebProjectRoleManager(RoleStore<Role> store)
            : base(store)
        {
        }

        public static WebProjectRoleManager Create(
                IdentityFactoryOptions<WebProjectRoleManager> options,
                IOwinContext context)
        {
            return new WebProjectRoleManager(new
                RoleStore<Role>(context.Get<WebProjectDbContext>()));
        }
    }
}