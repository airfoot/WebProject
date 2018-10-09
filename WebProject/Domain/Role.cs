using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebProject.Domain
{
    public class Role : IdentityRole
    {

        public Role() : base() { }

        public Role(string roleName) : base(roleName) { }
    }
}