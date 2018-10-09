using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class LeaveApplicationViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "开始时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public DateTime StartDateTime { get; set; }
        [Required]
      //  [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        [Display(Name = "结束时间")]
        public DateTime EndDateTime { get; set; }
        [Display(Name = "请假事由")]
        public string Description { get; set; }
        [Display(Name = "备注")]
        public string Comment { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "请假审批人")]
        public string ApproverEmail { get; set; }

        public float TotalDays { get; set; }
        public string InitiatorId { get; set; }
        public TaskState TaskState { get; set; }
        public LeaveType LeaveType { get; set; }
        public string InitiatorFullName { get; set; }

    }
}