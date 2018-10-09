using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Infrastructure
{
    public class AnnualLeaveStatisticHelper
    {
        //Caculate user's initial total annual leave days according to company policy
        public static int CaculateTotalDays(DateTime hireDate, int annualLeaveInitialDays, int annualLeaveMaxDays)
        {
            int totalDays = 0;
            //The last day of the year when user joins the company
            DateTime theLastDay = new DateTime(hireDate.Year, 12, 31);

            //The number of month from user joins the company to the last day of the year


            //
            if (theLastDay >= DateTime.Now)
            {
                DateTime tempDate = hireDate;
                double months = -1;
                do
                {
                    months++;
                    tempDate = tempDate.AddMonths(1);

                } while (tempDate >= theLastDay);

                totalDays = (int)Math.Floor((months / 12) * annualLeaveInitialDays);

            }
            else
            {
                DateTime tempDate = theLastDay;
                int years = 0;
                do
                {
                    years++;
                    tempDate = tempDate.AddYears(1);
                    if ( years >= annualLeaveMaxDays)
                    {
                        break;
                    }
                } while (tempDate <= DateTime.Now);
                totalDays = annualLeaveInitialDays + years;
                if (totalDays >= annualLeaveMaxDays)
                {
                    totalDays = annualLeaveMaxDays;
                }
            }
            return totalDays;
        }
    }
}