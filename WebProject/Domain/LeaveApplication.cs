using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject.Infrastructure;

namespace WebProject.Domain
{
    //public enum TaskState
    //{
    //    Applying,
    //    Approved,
    //    Refused,
    //    Canceled,
    //    Completed,
    //    Starting

    //}
    //public enum LeaveType
    //{
    //    AnnualLeave
    //}

    public class LeaveApplication
    {
        [Key]
        public Guid Id { get; set; }       
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Discription { get; set; }      
        public string Comment { get; set; }
        public TaskState TaskState { get; set; }
        public float TotalDays { get; set; }
        public LeaveType LeaveType { get; set; }
        public string InitiatorFK { get; set; }
        public string ApproverFK { get; set; }
        public virtual User Initiator { get; set; }
        public virtual User Approver { get; set; }

        //public LeaveApplication(User initiator, DateTime startDate, DateTime endDate, string description, string Comment, TaskState taskState,  LeaveType leaveType, Approver approver )
        //{

        //}
    }
}