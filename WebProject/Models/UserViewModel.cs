using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class UserViewModel
    {
        [Display(Name = "职务")]
        public string Title { get; set; }     
        [Display(Name = "部门")]
        public string Department { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "主管邮箱")]
        public string DirectManagerEmail { get; set; }
        [Display(Name = "证件号码")]
        public string IdentityNumber { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "联系地址")]
        public string ContactAddress { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }
        [Required]
        [Display(Name = "用户全名")]
        public string FullName { get; set; }
        [Required]       
        [Display(Name = "登陆账号")]
        public string UserName { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "登陆密码")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "入职时间")]
        public DateTime HireDate { get; set; }
        public string Id { get; set; }
        
    }
}