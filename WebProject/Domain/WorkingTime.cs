using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Domain
{
    public class WorkingTime
    {
        public int Id { get; set; }
        //Just Time part of below members is valid, Date part is invalid
        public DateTime StartTime { get; set; }
        public DateTime LunchBreakStartTime { get; set; }
        public DateTime LunchBreakEndTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}