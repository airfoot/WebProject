using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProject.Infrastructure;
using WebProject.Domain;
namespace WebProject.Domain.Services
{
    /// <summary>
    /// It will execute all of operating jobs for managing Meeting
    /// </summary>
    public interface IMeetingService
    {
        ReturnResult CreatingMeeting(Meeting meeting);
        ReturnResult ModifyMeeting(Meeting meeting);
        ReturnResult CancelMeeting(string meetingId);
        bool CanEditMeeting(string meetingId, string userId);
        IQueryable<Meeting> GetMeetingByMeetingRoom(DateTime startDate,DateTime endDate, string meetingRoomName);
        ICollection<Meeting> GetMeetingByUser(string userId);
    }
}
