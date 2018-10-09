using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Domain
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public DateTime BeginningTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MeetingSubject { get; set; }
        public string MeetingContent { get; set; }
        public bool IsCanceled { get; set; }
        public virtual User Owner { get; set; }
        public virtual Attendee Attendee { get; set; }
        public virtual MeetingRoom MeetingRoom { get; set; }
    }
}