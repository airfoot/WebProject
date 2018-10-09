using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class MeetingViewModel
    {

        [Display(Name = "会议发起人")]
        public string OwnerEmail { get; set; }
        [Display(Name = "会议室")]
        public string MeetingRoomName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "会议开始时间")]
        public DateTime StartDateTime { get; set; }
 
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "会议结束时间")]
        public DateTime EndDateTime { get; set; }
        [Required]
        [Display(Name = "会议主题")]
        public string MeetingSubject { get; set; }
        [Display(Name = "会议成员")]
        public string AttendeeEmails { get; set; }
        [Display(Name = "会议内容")]
        public string MeetingContent { get; set; }
    }
}