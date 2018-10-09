using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Domain;
using WebProject.Domain.Services;
using WebProject.Infrastructure;
using WebProject.Models;
using Microsoft.AspNet.Identity;
using X.PagedList;

namespace WebProject.Controllers
{
    public class LeaveController : Controller
    {
        private ILogger _logger;
        private WebProjectDbContext _context;
        private ILeaveApplicationService _leaveApplicationService;

        public LeaveController(ILogger logger, WebProjectDbContext webProjectDbContext, ILeaveApplicationService leaveApplicationService)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
            this._leaveApplicationService = leaveApplicationService;
        }
        // GET: Leave
        public ActionResult MyAnnualLeave(int? page)
        {
            string currentUserId = User.Identity.GetUserId();
            var leaveApplications = from leave in _context.LeaveApplications
                                    where leave.Initiator.Id == currentUserId
                                    orderby leave.StartDate descending
                                    select new LeaveApplicationViewModel()
                                    {
                                        Id = leave.Id.ToString(),
                                        StartDateTime = leave.StartDate,
                                        EndDateTime = leave.EndDate,
                                        TotalDays = leave.TotalDays,
                                        ApproverEmail = leave.Approver.Email,
                                        Description = leave.Discription,
                                        Comment = leave.Comment,
                                        TaskState = leave.TaskState,
                                        LeaveType = leave.LeaveType,
                                        InitiatorId = leave.Initiator.Id
                                    };
            var pageNumber = page ?? 1;
            var onePageOfLeaveApplications = leaveApplications.ToPagedList(pageNumber, 15);

            return View(onePageOfLeaveApplications);
        }
        [HttpGet]
        public ActionResult AnnualLeaveApplication()
        {
            LeaveApplicationViewModel leaveApplicationViewModel = new LeaveApplicationViewModel();
            leaveApplicationViewModel.ApproverEmail = User.Identity.GetUserManagerEmail();
            leaveApplicationViewModel.StartDateTime = DateTime.Now;
            leaveApplicationViewModel.EndDateTime = DateTime.Now;
            return View(leaveApplicationViewModel);
        }
        [HttpPost]
        public ActionResult AnnualLeaveApplication(LeaveApplicationViewModel leaveApplicationViewModel)
        {
            LeaveApplication leaveApplication = null;
            if(!ModelState.IsValid)
            {
                return View(leaveApplicationViewModel);
            }

            try
            {
                LeaveApplicationFactory leaveApplicationFactory = new LeaveApplicationFactory(_context);
                string userId = HttpContext.User.Identity.GetUserId();
                leaveApplication = leaveApplicationFactory.Create(userId, leaveApplicationViewModel.StartDateTime, leaveApplicationViewModel.EndDateTime, leaveApplicationViewModel.Description, leaveApplicationViewModel.ApproverEmail, leaveApplicationViewModel.Comment, LeaveType.AnnualLeave);
                _leaveApplicationService.CreateLeaveApplication(leaveApplication);
                return RedirectToAction("MyAnnualLeave");
            }
            catch(InvalidOperationException invalidException)
            {
                ModelState.AddModelError("", invalidException.Message);
            }catch(Exception e)
            {
                _logger.Error("Failed to create a new Annual Leave application", e);
                ModelState.AddModelError("", e.Message);
            }
            return View(leaveApplicationViewModel);
        }

        [HttpGet]
        public ActionResult ModifyAnnualLeave(string leaveApplicationId)
        {
            string currentUserId = User.Identity.GetUserId();
            var leaveApplication = _context.LeaveApplications.Where(l => l.Id.ToString() == leaveApplicationId).FirstOrDefault();
            LeaveApplicationViewModel leaveApplicationViewModel = new LeaveApplicationViewModel()
            {
                Id = leaveApplication.Id.ToString(),
                StartDateTime = leaveApplication.StartDate,
                EndDateTime = leaveApplication.EndDate,
                TotalDays = leaveApplication.TotalDays,
                ApproverEmail = leaveApplication.Approver.Email,
                Description = leaveApplication.Discription,
                Comment = leaveApplication.Comment,
                TaskState = leaveApplication.TaskState,
                LeaveType = leaveApplication.LeaveType,
                InitiatorId = leaveApplication.Initiator.Id
            };
            return View(leaveApplicationViewModel);
        }

        [HttpPost]
        public ActionResult ModifyAnnualLeave(LeaveApplicationViewModel leaveApplicationViewModel,string leaveApplicationId)
        {
            if (!ModelState.IsValid)
            {
                return View(leaveApplicationViewModel);
            }

            try
            {
                LeaveApplication leaveApplication = _context.LeaveApplications.Find(new Guid(leaveApplicationId));
                leaveApplication.StartDate = leaveApplicationViewModel.StartDateTime;
                leaveApplication.EndDate = leaveApplicationViewModel.EndDateTime;
                leaveApplication.Discription = leaveApplicationViewModel.Description;
                leaveApplication.Comment = leaveApplicationViewModel.Comment;
                User user = _context.Users.Find(HttpContext.User.Identity.GetUserId());
                if(!LeaveApplicationHelper.CanApprove(user, leaveApplicationViewModel.ApproverEmail))
                {
                    throw new InvalidOperationException("请填写正确的审批人邮箱地址");
                }
                User approver = _context.Users.Where(u => u.Email == leaveApplicationViewModel.ApproverEmail).FirstOrDefault();
                leaveApplication.Approver = approver;
                leaveApplication.TotalDays = LeaveApplicationHelper.CaculateTotalDays(leaveApplicationViewModel.StartDateTime, leaveApplicationViewModel.EndDateTime, _context);
                leaveApplication.TaskState = TaskState.Applying;
                _leaveApplicationService.ModifyLeaveApplication(leaveApplication);
                return RedirectToAction("MyAnnualLeave");
            }
            catch (InvalidOperationException invalidException)
            {
                ModelState.AddModelError("", invalidException.Message);
            }
            catch (Exception e)
            {
                _logger.Error("Failed to create a new Annual Leave application", e);
                ModelState.AddModelError("", e.Message);
            }
            return View(leaveApplicationViewModel);

        }

        public ActionResult CancelAnnualLeave(string leaveApplicationId)
        {
            try
            { 
            LeaveApplication leaveApplication = _context.LeaveApplications.Find(new Guid(leaveApplicationId));
            leaveApplication.TaskState = TaskState.Canceled;
            _leaveApplicationService.CancelLeaveApplication(leaveApplication);
            return RedirectToAction("MyAnnualLeave");
            }
            catch(Exception e)
            {
                _logger.Error("Failed to cancel an Annual Leave application", e);
            }
            return RedirectToAction("MyAnnualLeave");
        }

        public ActionResult MyApprovingAnnualLeave(int? page)
        {

            string currentUserId = User.Identity.GetUserId();
            var approvingLeaveApplications = from leave in _context.LeaveApplications
                                             where leave.Approver.Id == currentUserId && leave.TaskState == TaskState.Applying
                                             orderby leave.StartDate descending
                                             select new LeaveApplicationViewModel()
                                             {
                                                 Id = leave.Id.ToString(),
                                                 StartDateTime = leave.StartDate,
                                                 EndDateTime = leave.EndDate,
                                                 TotalDays = leave.TotalDays,
                                                 ApproverEmail = leave.Approver.Email,
                                                 Description = leave.Discription,
                                                 Comment = leave.Comment,
                                                 TaskState = leave.TaskState,
                                                 LeaveType = leave.LeaveType,
                                                 InitiatorId = leave.Initiator.Id,
                                                 InitiatorFullName = leave.Initiator.FullName,
                                    };
            var pageNumber = page ?? 1;
            var onePageOfApprovingLeaves = approvingLeaveApplications.ToPagedList(pageNumber, 15);

            return View(onePageOfApprovingLeaves);
        }

        [HttpGet]
        public ActionResult RefuseAnnualLeaveApplicationForm(string leaveApplicationId)
        {
            var leaveApplication = _context.LeaveApplications.Where(l => l.Id.ToString() == leaveApplicationId).FirstOrDefault();
            LeaveApplicationViewModel leaveApplicationViewModel = new LeaveApplicationViewModel()
            {
                Id = leaveApplication.Id.ToString(),
                StartDateTime = leaveApplication.StartDate,
                EndDateTime = leaveApplication.EndDate,
                TotalDays = leaveApplication.TotalDays,
                ApproverEmail = leaveApplication.Approver.Email,
                Description = leaveApplication.Discription,
                Comment = leaveApplication.Comment,
                TaskState = leaveApplication.TaskState,
                LeaveType = leaveApplication.LeaveType,
                InitiatorId = leaveApplication.Initiator.Id
            };
            return PartialView(leaveApplicationViewModel);
        }

        [HttpPost]
        public ActionResult RefuseAnnualLeaveApplication(LeaveApplicationViewModel leaveApplicationViewModel,string leaveApplicationId)
        {
            LeaveApplication leaveApplication = _context.LeaveApplications.Find(new Guid(leaveApplicationId));
            leaveApplication.Comment = leaveApplicationViewModel.Comment;
            string currentUserId = HttpContext.User.Identity.GetUserId();
            User approver = _context.Users.Find(currentUserId);
            _leaveApplicationService.RefuseLeaveApplication(leaveApplication, approver);

            return RedirectToAction("MyApprovingAnnualLeave");
        }

        public ActionResult ApproveAnnualLeaveApplication(string leaveApplicationId)
        {
            LeaveApplication leaveApplication = _context.LeaveApplications.Find(new Guid(leaveApplicationId));
            leaveApplication.TaskState = TaskState.Approved;
            string currentUserId = HttpContext.User.Identity.GetUserId();
            User approver = _context.Users.Find(currentUserId);
            _leaveApplicationService.ApproveLeaveApplication(leaveApplication, approver);
            return RedirectToAction("MyApprovingAnnualLeave");
        }
    
    }
}