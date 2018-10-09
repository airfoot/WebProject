using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class LoginLogViewModel
    {
        public string Id { get; set; }
        [Display(Name = "登陆用户")]
        public string UserName { get; set; }
        [Display(Name = "用户ID")]
        public string UserId { get; set; }
        [Display(Name = "登陆时间")]
        public DateTime LoginDateTime { get; set; }
        [Display(Name = "用户IP地址")]
        public string RemoteIP { get; set; }
    }
}