using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Microsoft.AspNet.Identity;
using WebProject.Domain;
using WebProject.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace WebProject.Domain
{
    public class Attendee
    {
        public Attendee()
        {
            this.Users = new HashSet<User>();
        }
        public Guid Id { get; set; }      
        public virtual ICollection<User> Users { get; set; }
        public string ExternalUserMailAddresses { get; set; }

        public string GetAttendeeMailAddresses()
        {
            StringBuilder mailAddress = new StringBuilder();
            foreach (var user in this.Users)
            {
                mailAddress.Append(user.Email + "; ");
            }
            mailAddress.Append(this.ExternalUserMailAddresses);

            return mailAddress.ToString();
        }

        public void SetAttendeeMailAddresses(string mailAddresses,WebProjectDbContext webProjectDbContext)
        {
            StringBuilder tempAddresses = new StringBuilder();

            if (string.IsNullOrEmpty(mailAddresses))
            {
                throw new InvalidOperationException("E-Mail Address cannot be empty");
            }

            string[] arrayAddresses = mailAddresses.Split(';');

            foreach (var address in arrayAddresses)
            {
                string tempAddress = address.Trim(' ');
                User user = webProjectDbContext.Users.Where(u => u.Email == tempAddress).FirstOrDefault();
                if (user == null)
                {
                    tempAddresses.Append(tempAddress + "; ");
                }
                else
                {
                    this.Users.Add(user);
                }
            }

            this.ExternalUserMailAddresses = tempAddresses.ToString();

        }
    }
}