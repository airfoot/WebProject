using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using WebProject.Models;
using WebProject.Models.Menu;
using WebProject.Infrastructure;
using WebProject.Domain;
using WebProject.Domain.Services;
using Microsoft.AspNet.Identity;

namespace WebProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ILogger _logger;
        private WebProjectDbContext _context;
        private IMeetingService _meetingService;

        public HomeController(ILogger logger, WebProjectDbContext webProjectDbContext, IMeetingService meetingService)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
            this._meetingService = meetingService;
        }
        // GET: Home
        public ActionResult Index()
        {
            IEnumerable<Meeting> myMeetings = _meetingService.GetMeetingByUser(HttpContext.User.Identity.GetUserId());
            var myMeetingViewModels = (from m in myMeetings
                                       orderby m.BeginningTime descending
                                       select new MeetingViewModel()
                                       {
                                           OwnerEmail = m.Owner.Email,
                                           MeetingRoomName = m.MeetingRoom.Name,
                                           StartDateTime = m.BeginningTime,
                                           EndDateTime = m.EndTime,
                                           MeetingContent = m.MeetingContent,
                                           MeetingSubject = m.MeetingSubject,
                                           AttendeeEmails = m.Attendee.GetAttendeeMailAddresses(),
                                       }).Take(5);
            ViewBag.MyMeetings = myMeetingViewModels;

            //Transfer My AnnualLeave application to View
            string currentUserId = User.Identity.GetUserId();
            var leaveApplications = (from leave in _context.LeaveApplications
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
                                     }).Take(5);
            ViewBag.MyAnnualLeaveApplications = leaveApplications;

            if(User.IsInRole("Manager") || User.IsInRole("Admin"))
            {
                var approvingLeaveApplications = (from leave in _context.LeaveApplications
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
                                                 }).Take(5);
                ViewBag.MyApprovingAnnualLeaves = approvingLeaveApplications;
            }

            return View();
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult Menu()
        {
            List < MenuInformation > menuList = new List<MenuInformation>();
            Menu menu = new Menu();
            IPrincipal User = HttpContext.User;
            if(User.IsInRole("Admin"))
            {
                menuList = menu.GetAdminMenu();
            }
            else if(User.IsInRole("Manager"))
            {
                menuList = menu.GetManagerMenu();
            }
            else
            {
                menuList = menu.GetUserRoleMenu();
            }

            return PartialView(menuList);
        }
    }
}