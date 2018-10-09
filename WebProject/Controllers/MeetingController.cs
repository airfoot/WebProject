using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Infrastructure;
using WebProject.Domain;
using WebProject.Domain.Services;
using WebProject.Models;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using X.PagedList;

namespace WebProject.Controllers
{

    [Authorize]
    public class MeetingController : Controller
    {
        private ILogger _logger;
        private WebProjectDbContext _context;
        private IMeetingService _meetingService;

        public MeetingController(ILogger logger, WebProjectDbContext webProjectDbContext, IMeetingService meetingService)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
            this._meetingService = meetingService;
        }
        public ActionResult MyMeetings(int? page)
        {
            IEnumerable<Meeting> myMeetings = _meetingService.GetMeetingByUser(HttpContext.User.Identity.GetUserId());
            var myMeetingViewModels = from m in myMeetings
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
                                      };
            var pageNumber = page ?? 1;
            var onePageOfMyMeeting = myMeetingViewModels.ToPagedList(pageNumber, 15);

            return View(onePageOfMyMeeting);
        }

        /// <summary>
        /// 用户点击页面左侧导航菜单中的相应会议室时，返回会议室预定情况
        /// </summary>
        /// <param name="Id">参数Id是MeetingRoom会议室的Id</param>
        /// <returns></returns>
        public ActionResult MeetingRoom(string Id)
        {
            MeetingRoom meetingRoom = _context.MeetingRooms.Find(Id);
            ViewBag.MeetingName = meetingRoom.Name;
            ViewBag.MeetingRoomId = Id;
            return View();
        }

        [HttpPost]
        public ActionResult CreateMeetingForm(string meetingRoomId)
        {
            MeetingRoom meetingRoom = _context.MeetingRooms.Find(meetingRoomId);
            string meetingRoomName = meetingRoom.Name;
            ViewBag.MeetingRoomId = meetingRoomId;
            MeetingViewModel meetingViewModel = new MeetingViewModel();
            meetingViewModel.OwnerEmail = User.Identity.GetUserEmail();
            meetingViewModel.MeetingRoomName = meetingRoomName;
            meetingViewModel.EndDateTime = DateTime.Now;
            meetingViewModel.StartDateTime = DateTime.Now;
            return PartialView(meetingViewModel);
        }

        public ActionResult CreateMeeting(MeetingViewModel meetingViewModel, string meetingRoomId)
        {
            Meeting meeting=null;
            if (ModelState.IsValid)
            {
                ReturnResult result = new ReturnResult();
                try
                {
                    meeting = new Meeting();
                    Attendee attendee = new Attendee();
                    attendee.Id = Guid.NewGuid();
                    //解析用户输入的与会人邮箱地址，然后构建attendee，此处没有处理用户错误输入例外
                    attendee.SetAttendeeMailAddresses(meetingViewModel.AttendeeEmails, _context);
                    User owner = _context.Users.Find(HttpContext.User.Identity.GetUserId());
                    MeetingRoom meetingRoom = _context.MeetingRooms.Find(meetingRoomId);
                    //下面是使用输入内容初始化新会议
                    meeting.Id = Guid.NewGuid();
                    meeting.Owner = owner;
                    meeting.BeginningTime = meetingViewModel.StartDateTime;
                    meeting.EndTime = meetingViewModel.EndDateTime;
                    meeting.MeetingSubject = meetingViewModel.MeetingSubject;
                    meeting.MeetingRoom = meetingRoom;
                    meeting.MeetingContent = meetingViewModel.MeetingContent;
                    meeting.Attendee = attendee;
                    meeting.IsCanceled = false;

                    result = _meetingService.CreatingMeeting(meeting);
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    ModelState.AddModelError("", invalidOperationException.Message);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message, e);
                }


                if (result.Result)
                {
                    MeetingEvent meetingEvent = new MeetingEvent();
                    meetingEvent.Id = meeting.Id.ToString();
                    meetingEvent.Title = meeting.MeetingSubject;
                    meetingEvent.StartDateTime = meeting.BeginningTime;
                    meetingEvent.EndDateTime = meeting.EndTime;

                    var returnSuccess = new { isSuccess = true, eventdetail = meetingEvent };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnSuccess), "application/json");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }

            }
            var returnFail = new
            {
                isSuccess = false,
                // ModelState错误信息
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
                  .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnFail), "application/json");
        }

        public ActionResult GetMeetingEvents(string meetingRoomId, DateTime start, DateTime end)
        {
            var queryMeetings = _meetingService.GetMeetingByMeetingRoom(start, end, meetingRoomId);
            //下面新建的匿名对象必须严格按照如下所示成员名称小写
            var meetingEvents = from m in queryMeetings                            
                               select new
                                   {
                                       id = m.Id,
                                       title = m.MeetingSubject,
                                       start = m.BeginningTime,
                                       end = m.EndTime

                                   };

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(meetingEvents), "application/json");
       
        }

        
        public ActionResult ModifyMeetingForm(string meetingId)
        {
            ViewBag.MeetingId = meetingId;
            bool canEditMeeting = _meetingService.CanEditMeeting(meetingId, User.Identity.GetUserId());
            ViewBag.CanEditMeeting = canEditMeeting;
            Meeting meeting = _context.Meetings.Find(new Guid(meetingId));
            MeetingViewModel meetingViewModel = new MeetingViewModel();

            meetingViewModel.OwnerEmail = meeting.Owner.Email;
            meetingViewModel.MeetingRoomName = meeting.MeetingRoom.Name;
            meetingViewModel.StartDateTime = meeting.BeginningTime;
            meetingViewModel.EndDateTime = meeting.EndTime;
            meetingViewModel.MeetingSubject = meeting.MeetingSubject;
            meetingViewModel.MeetingContent = meeting.MeetingContent;
            meetingViewModel.AttendeeEmails = meeting.Attendee.GetAttendeeMailAddresses();

            return PartialView(meetingViewModel);
        }

        [HttpPost]
        public ActionResult ModifyMeeting(MeetingViewModel meetingViewModel, string meetingId)
        {
            Meeting meeting = null;
            ReturnResult result = new ReturnResult();
            if (ModelState.IsValid)
            {
                meeting = _context.Meetings.Find(new Guid(meetingId));
                Attendee attendee = meeting.Attendee;
                attendee.SetAttendeeMailAddresses(meetingViewModel.AttendeeEmails, _context);
                meeting.Attendee = attendee;
                meeting.BeginningTime = meetingViewModel.StartDateTime;
                meeting.EndTime = meetingViewModel.EndDateTime;
                meeting.MeetingSubject = meetingViewModel.MeetingSubject;
                meeting.MeetingContent = meetingViewModel.MeetingContent;

                result = _meetingService.ModifyMeeting(meeting);
                if (result.Result)
                {
                    MeetingEvent meetingEvent = new MeetingEvent();
                    meetingEvent.Id = meeting.Id.ToString();
                    meetingEvent.Title = meeting.MeetingSubject;
                    meetingEvent.StartDateTime = meeting.BeginningTime;
                    meetingEvent.EndDateTime = meeting.EndTime;

                    var returnSuccess = new { isSuccess = true, eventdetail = meetingEvent };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnSuccess), "application/json");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            var returnFail = new
            {
                isSuccess = false,
                // ModelState错误信息
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
                 .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnFail), "application/json");
        }

        [HttpPost]
        public ActionResult CancelMeeting(string meetingId)
        {
            ReturnResult result = _meetingService.CancelMeeting(meetingId);
            if(result.Result)
            {
                var returnSuccess = new { isSuccess = true};
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnSuccess), "application/json");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
            }
            var returnFail = new
            {
                isSuccess = false,
                // ModelState错误信息
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
                 .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnFail), "application/json");
        }

    }
}