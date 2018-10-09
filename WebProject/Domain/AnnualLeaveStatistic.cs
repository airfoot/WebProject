using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;

namespace WebProject.Domain
{
    public class AnnualLeaveStatistic
    { 
        public Guid Id { get; set; }
        public User User { get; set; }
        //Every Employee has default number of paid leave defined by labour law, the number of paid leave will increase as years of working for the company increase
        public float TotalDays { get; set; }
        public float UsedDays { get; set; }
        public float RemainingDays { get; set; }
        public int TransferringDays { get; set; }
        //Default number of paid annual leave
        public int DefaultDays { get; set; }
            
        
    }
}