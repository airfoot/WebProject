using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebProject.Domain
{
    public class User : IdentityUser 
    {
       
        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public string ContactAddress { get; set; }
        public string DirectManagerEmail { get; set; }
        public string Department { get; set; }
        public string UserIconURL { get; set; }
        public string Title { get; set; }
        //It is on board date of new employee
       // [Column("HireDate",TypeName = "datetime2")]
        public DateTime HireDate { get; set; }
    }
}