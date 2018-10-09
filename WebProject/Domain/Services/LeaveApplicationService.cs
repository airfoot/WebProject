using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;
using WebProject.Domain;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace WebProject.Domain.Services
{
    public class LeaveApplicationService : ILeaveApplicationService
    {

        private ILogger _logger;
        private WebProjectDbContext _context;
        public LeaveApplicationService(ILogger logger, WebProjectDbContext webProjectDbContext)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
        }
        public void CreateLeaveApplication(LeaveApplication leaveApplication)
        {
            if(IsScheduleConflict(leaveApplication))
            {
                throw new InvalidOperationException("当前请假时间和已存在的请假申请冲突");
            }
            AnnualLeaveStatistic annualLeaveStatistic = GetAnnualLeaveStatistic(leaveApplication.Initiator);
            if(leaveApplication.TotalDays > annualLeaveStatistic.RemainingDays)
            {
                throw new InvalidOperationException("剩余假期不足，请重新选择请假时间");
            }
            annualLeaveStatistic.RemainingDays = annualLeaveStatistic.RemainingDays - leaveApplication.TotalDays;
            leaveApplication.TaskState = TaskState.Applying;
            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.LeaveApplications.Add(leaveApplication);
                    _context.AnnualLeaveStatistics.Attach(annualLeaveStatistic);
                    _context.Entry(annualLeaveStatistic).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    _logger.Error(e.Message, e);
                    throw;
                }
            }

        }

        public void ModifyLeaveApplication(LeaveApplication leaveApplication)
        {
            if (IsScheduleConflict(leaveApplication))
            {
                throw new InvalidOperationException("当前请假时间和已存在的请假申请冲突");
            }
            AnnualLeaveStatistic annualLeaveStatistic = GetAnnualLeaveStatistic(leaveApplication.Initiator);
            LeaveApplication oldLeaveApplication = _context.LeaveApplications.Find(leaveApplication.Id);
            float differenceTotalDays = leaveApplication.TotalDays - oldLeaveApplication.TotalDays;
            annualLeaveStatistic.RemainingDays = annualLeaveStatistic.RemainingDays + differenceTotalDays;
            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.LeaveApplications.Attach(leaveApplication);
                    _context.Entry(leaveApplication).State = EntityState.Modified;
                    _context.AnnualLeaveStatistics.Attach(annualLeaveStatistic);
                    _context.Entry(annualLeaveStatistic).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.Error(e.Message, e);
                    throw;
                }
            }

        }

        public void CancelLeaveApplication(LeaveApplication leaveApplication)
        {
            if(leaveApplication.TaskState == TaskState.Approved)
            {
                throw new InvalidOperationException("审批完成的申请不可以取消或删除");
            }
            AnnualLeaveStatistic annualLeaveStatistic = GetAnnualLeaveStatistic(leaveApplication.Initiator);
            annualLeaveStatistic.RemainingDays = annualLeaveStatistic.RemainingDays + leaveApplication.TotalDays;

            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.LeaveApplications.Attach(leaveApplication);
                    _context.Entry(leaveApplication).State = EntityState.Modified;
                    _context.AnnualLeaveStatistics.Attach(annualLeaveStatistic);
                    _context.Entry(annualLeaveStatistic).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.Error(e.Message, e);
                    throw;
                }
            }
        }

        public void ApproveLeaveApplication(LeaveApplication leaveApplication, User user)
        {
            if(!HasPrivilege(leaveApplication,user))
            {
                throw new InvalidOperationException("用户没有权限管理此申请");
            }
            leaveApplication.TaskState = TaskState.Approved;

            _context.LeaveApplications.Attach(leaveApplication);
            _context.Entry(leaveApplication).State = EntityState.Modified;
            _context.SaveChanges();

        }

        public void RefuseLeaveApplication(LeaveApplication leaveApplication, User user)
        {
            if (!HasPrivilege(leaveApplication, user))
            {
                throw new InvalidOperationException("用户没有权限管理此申请");
            }

            if (leaveApplication.TaskState == TaskState.Approved)
            {
                throw new InvalidOperationException("审批完成的申请不可以取消或删除");
            }
            AnnualLeaveStatistic annualLeaveStatistic = GetAnnualLeaveStatistic(leaveApplication.Initiator);
            annualLeaveStatistic.RemainingDays = annualLeaveStatistic.RemainingDays + leaveApplication.TotalDays;
            leaveApplication.TaskState = TaskState.Refused;
            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.LeaveApplications.Attach(leaveApplication);
                    _context.Entry(leaveApplication).State = EntityState.Modified;
                    _context.AnnualLeaveStatistics.Attach(annualLeaveStatistic);
                    _context.Entry(annualLeaveStatistic).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.Error(e.Message, e);
                    throw;
                }
            }

        }

        /// <summary>
        /// 获取指定用户创建的休假申请
        /// </summary>
        /// <param name="user">申请人.</param>
        /// <returns></returns>
        public IQueryable<LeaveApplication> GetLeaveApplicationsByUser(User user)
        {
            var result = from l in _context.LeaveApplications
                    where l.Initiator.Id == user.Id
                    select l;
            return result;

        }

        /// <summary>
        /// 获取需要用户审批的休假申请.
        /// </summary>
        /// <param name="user">审批人.</param>
        /// <returns></returns>
        public IQueryable<LeaveApplication> GetApprovingApplicationsByUser(User user)
        {
            var result = from l in _context.LeaveApplications
                         where l.Approver.Id == user.Id
                         select l;
            return result;
        }


        private AnnualLeaveStatistic GetAnnualLeaveStatistic(User user)
        {
            AnnualLeaveStatistic annualLeaveStatistic = _context.AnnualLeaveStatistics.Where(a => a.User.Id == user.Id).FirstOrDefault();
            if(annualLeaveStatistic == null)
            {
                AnnualLeaveStatisticFactory annualLeaveStatisticFactory = new AnnualLeaveStatisticFactory(_context);
                annualLeaveStatistic = annualLeaveStatisticFactory.CreateAndSave(user);
            }
            return annualLeaveStatistic;
        }

        /// <summary>
        /// 判断用户是否有权限批准或者拒绝休假申请.
        /// </summary>
        /// <param name="leaveApplication">The leave application.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <c>true</c><c>false</c>.
        /// </returns>
        private bool HasPrivilege(LeaveApplication leaveApplication, User user)
        {
            if(leaveApplication.Approver.Id == user.Id)
            {
                return true;
            }

            return false;
        }


        private bool IsScheduleConflict(LeaveApplication leaveApplication)
        {
            //The function does not judge a special situation, when user submit an short application(less than an hour) that it is located within the same hours with previous short application.
            //because the least granularity is one hour
            //The bug will be fixed later
            string currentUserId = HttpContext.Current.User.Identity.GetUserId();
            IEnumerable<LeaveApplication> leaveApplications = _context.LeaveApplications.Where(l=>l.Initiator.Id == currentUserId);
            foreach (var _leaveApplication in leaveApplications)
            {
                if(_leaveApplication.Id == leaveApplication.Id)
                {
                    continue;
                }
                if (_leaveApplication.TaskState == TaskState.Canceled)
                {
                    continue;
                }
                if(_leaveApplication.TaskState == TaskState.Refused)
                {
                    continue;
                }
                if (_leaveApplication.StartDate > leaveApplication.StartDate && _leaveApplication.StartDate < leaveApplication.EndDate)
                {
                    return true;
                }
                else if (_leaveApplication.EndDate > leaveApplication.StartDate && _leaveApplication.EndDate < leaveApplication.EndDate)
                {
                    return true;
                }
                else if (_leaveApplication.StartDate <= leaveApplication.StartDate && _leaveApplication.EndDate >= leaveApplication.EndDate)
                {
                    return true;
                }

            }

            return false;
        }

    }
}