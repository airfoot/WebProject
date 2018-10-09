using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Domain
{
    public class CompanyVacationPolicy
    {
        public int Id { get; set; }
        //represent the default sum of paid leave that every one is entitled, 
        public int AnnualLeaveInitialDays { get; set; }
        public int AnnualLeaveMaxDays { get; set; }
    }
}