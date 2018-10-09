using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Infrastructure;

namespace WebProject.Domain
{
    public class AnnualLeaveStatisticFactory
    {
        WebProjectDbContext context;

        public AnnualLeaveStatisticFactory(WebProjectDbContext context)
        {
            this.context = context;
        }

        public AnnualLeaveStatistic CreateAndSave(User user)
        {
            AnnualLeaveStatistic annualLeaveStatistic = new AnnualLeaveStatistic();
            CompanyVacationPolicy vacationPolicy = context.CompanmyVacationPolicies.First();
            int totalDays = 0;
            totalDays = AnnualLeaveStatisticHelper.CaculateTotalDays(user.HireDate, vacationPolicy.AnnualLeaveInitialDays, vacationPolicy.AnnualLeaveMaxDays);
            annualLeaveStatistic.Id = Guid.NewGuid();
            annualLeaveStatistic.RemainingDays = totalDays;
            annualLeaveStatistic.TotalDays = totalDays;
            annualLeaveStatistic.TransferringDays = 0;
            annualLeaveStatistic.UsedDays = 0;
            annualLeaveStatistic.DefaultDays = 0;
            annualLeaveStatistic.User = user;

            context.AnnualLeaveStatistics.Add(annualLeaveStatistic);
            context.SaveChanges();

            return annualLeaveStatistic;
        }

    }
}