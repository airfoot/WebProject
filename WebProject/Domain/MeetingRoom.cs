using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Domain
{
    public class MeetingRoom
    {
        public MeetingRoom()
        {
            this.Meetings = new HashSet<Meeting>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsHaveingProjector { get; set; }
        public bool IsHavingTelephone { get; set; }
        public bool IsHavingVideoDevice { get; set; }
        public bool IsHavingVoiceDevice { get; set; }
        public string Size { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }

    }
}