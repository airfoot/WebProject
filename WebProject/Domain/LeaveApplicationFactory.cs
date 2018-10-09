using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace WebProject.Domain
{
    public class LeaveApplicationFactory
    {
        WebProjectDbContext _context;
       public LeaveApplicationFactory(WebProjectDbContext webProjectDbContext)
        {
            this._context = webProjectDbContext;
        }
        public LeaveApplication Create(string initiatorId, DateTime startDate, DateTime endDate, string description, string approverEmail, string comment, LeaveType leaveType)
        {
            User approver = null;
            float totalDays;
            User initiator = _context.Users.Find(initiatorId);
            LeaveApplication leaveApplication = new LeaveApplication();

            approver = _context.Users.Where(u => u.Email == approverEmail).FirstOrDefault();
            if (approver == null)
            {
                throw new InvalidOperationException(@"请填写正确的直属审批人的邮箱地址");
            }
            if (!LeaveApplicationHelper.CanApprove(initiator,approverEmail))
            {
                throw new InvalidOperationException(@"请填写正确的直属审批人的邮箱地址");
            }
            
            if(endDate <= startDate)
            {
                throw new InvalidOperationException(@"申请结束日期不可以早于等于申请开始日期");
            }

            totalDays = LeaveApplicationHelper.CaculateTotalDays(startDate, endDate,_context);
            leaveApplication.Initiator = initiator;
            leaveApplication.StartDate = startDate;
            leaveApplication.EndDate = endDate;
            leaveApplication.Discription = description;
            leaveApplication.Approver = approver;
            leaveApplication.Id = Guid.NewGuid();
            leaveApplication.Comment = comment;
            leaveApplication.TaskState = TaskState.Starting;
            leaveApplication.TotalDays = totalDays;
            leaveApplication.LeaveType = LeaveType.AnnualLeave;

            return leaveApplication;

        }

    }
}