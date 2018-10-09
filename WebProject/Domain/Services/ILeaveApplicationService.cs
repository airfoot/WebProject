using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProject.Domain;
using WebProject.Infrastructure;

namespace WebProject.Domain.Services
{
   public interface ILeaveApplicationService
    {
        void CreateLeaveApplication(LeaveApplication leaveApplication);
        void ModifyLeaveApplication(LeaveApplication leaveApplication);
        void CancelLeaveApplication(LeaveApplication leaveApplication);
        void ApproveLeaveApplication(LeaveApplication leaveApplication, User user);
        void RefuseLeaveApplication(LeaveApplication leaveApplication, User user);
        IQueryable<LeaveApplication> GetLeaveApplicationsByUser(User user);
        IQueryable<LeaveApplication> GetApprovingApplicationsByUser(User user);
        
    }
}
