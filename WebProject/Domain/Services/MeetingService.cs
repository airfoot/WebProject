using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;
using WebProject.Domain;
using System.Threading;
using System.Data.Entity;

namespace WebProject.Domain.Services
{

    public class MeetingService : IMeetingService
    {
        private ILogger _logger;
        private WebProjectDbContext _context;
        private Object _locker = new Object();
        /// <summary>
        /// Will be used to initialize a new instance of the class. Parameters will be injected by IOC container
        /// </summary>
        /// <param name="logger">The logger.</param>
        public MeetingService(ILogger logger, WebProjectDbContext webProjectDbContext)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
        }


        private bool IsScheduleConflict(Meeting meeting)
        {
            IEnumerable<Meeting> meetings = _context.Meetings.Where(m => m.MeetingRoom.Id == meeting.MeetingRoom.Id);
            foreach( var _meeting in meetings)
            { 
                if(_meeting.Id == meeting.Id)
                {
                    continue;
                }
                if(_meeting.IsCanceled)
                {
                    continue;
                }
                 if(_meeting.BeginningTime > meeting.BeginningTime && _meeting.BeginningTime < meeting.EndTime)
                {
                    return true;
                }
                else if(_meeting.EndTime > meeting.BeginningTime && _meeting.EndTime < meeting.EndTime)
                {
                    return true;
                }
                else if(_meeting.BeginningTime <= meeting.BeginningTime && _meeting.EndTime >= meeting.EndTime)
                {
                    return true;
                }

            }

           return false;
        }

   

        public ReturnResult CreatingMeeting(Meeting meeting)
        {
            ReturnResult returnResult = new ReturnResult();
            if(meeting.BeginningTime>=meeting.EndTime)
            {
                returnResult.Result = false;
                returnResult.Message = "会议开始时间不能大于等于结束时间";
                return returnResult;
            }

            if(!IsScheduleConflict(meeting))
            {
                lock(_locker)
                {
                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            _context.Attendees.Add(meeting.Attendee);
                            _context.Meetings.Add(meeting);
                            _context.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            returnResult.Result = false;
                            returnResult.Message = e.Message;
                            _logger.Error("Faild to save a new Meeting", e);
                            return returnResult;
                        }
                    }
                }
            }
            else
            {
                returnResult.Result = false;
                returnResult.Message = "会议预定时间冲突";
                return returnResult;
            }
            returnResult.Result = true;
            return returnResult;
        }
        public ReturnResult ModifyMeeting(Meeting meeting)
        {
            ReturnResult returnResult = new ReturnResult();
            if (!IsScheduleConflict(meeting))
            {
                lock (_locker)
                {

                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            _context.Attendees.Attach(meeting.Attendee);
                            _context.Entry(meeting.Attendee).State = EntityState.Modified;
                            _context.Meetings.Attach(meeting);
                            _context.Entry(meeting).State = EntityState.Modified;
                            _context.SaveChanges();
                            transaction.Commit();
                            returnResult.Result = true;
                            return returnResult;
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            returnResult.Result = false;
                            returnResult.Message = e.Message;
                            _logger.Error("Faild to modify a Meeting", e);
                            return returnResult;
                        }
                    }
                }
            }
            returnResult.Result = false;
            returnResult.Message = "会议时间冲突";
            return returnResult;
        }
        public ReturnResult CancelMeeting(string meetingId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                Meeting meeting = _context.Meetings.Find(new Guid(meetingId));
                meeting.IsCanceled = true;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                returnResult.Result = false;
                returnResult.Message = e.Message;
                _logger.Error("Faild to cancel a Meeting", e);
                return returnResult;
            }
            returnResult.Result = true;
            return returnResult;

        }
        public bool CanEditMeeting(string meetingId, string userId)
        {
            try
            {
                Meeting meeting = _context.Meetings.Find(new Guid(meetingId));
                if(meeting.Owner.Id == userId)
                {
                    return true;
                }

            }catch(Exception e)
            {
                _logger.Error(e.Message, e);
            }
            return false;
        }
        public IQueryable<Meeting> GetMeetingByMeetingRoom(DateTime startDate,DateTime endDate,string meetingRoomId)
        {
            MeetingRoom meetingRoom = new MeetingRoom();
            try
            {
                if (!String.IsNullOrEmpty(meetingRoomId))
                {
                    meetingRoom = _context.MeetingRooms.Find(meetingRoomId);
                }
            }catch (System.InvalidOperationException ex)
            {
                throw new InvalidOperationException("MeetingRoom is not correct",ex);
            }

            try
            { 
                var meetings = from m in _context.Meetings
                               where m.IsCanceled == false && m.MeetingRoom.Id == meetingRoom.Id && ((m.BeginningTime > startDate && m.BeginningTime < endDate) || ( m.EndTime > startDate && m.EndTime < endDate))
                               select m;

                return meetings;       
            }catch(Exception e)
            {
                 _logger.Error(e.Message, e);
            }

            return null;

        }


        /// <summary>
        /// Gets the meetings by user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns> Below method will exclude duplicate meeting  </returns>
        public ICollection<Meeting> GetMeetingByUser(string userId)
        {
            ICollection<Meeting> meetings = new List<Meeting>();
            foreach(var meeting in _context.Meetings)
            {
                if(meeting.Owner.Id == userId)
                {
                    meetings.Add(meeting);
                }
                foreach(var attendeeUser in meeting.Attendee.Users)
                {
                    if (attendeeUser.Id == userId)
                        if(!meetings.Contains(meeting))
                        {
                            meetings.Add(meeting);
                        }
                        
                }
            }
            return meetings;
        }



    }
}