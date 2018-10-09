using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Domain;

namespace WebProject.Infrastructure
{
    public class LeaveApplicationHelper
    {
        public static float CaculateTotalDays(DateTime startDate, DateTime endDate,WebProjectDbContext webProjectDbContext)
        {
            const int workingHours = 8;
            double totalMinutes = 0;
            float totalDays = 0;
            WebProjectDbContext context = webProjectDbContext;
            WorkingTime workingTime = context.WorkingTimes.First();
            if(endDate <= startDate)
            {
                throw new InvalidOperationException(@"开始日期不能小于结束日期");
            }

            //如果开始和结束日期是周六或者周天，抛出异常
            if(!IsWorkingDay(startDate) || !IsWorkingDay(endDate))
            {
                throw new InvalidOperationException(@"开始日期和结束日期不能是星期六和星期天等休假时间");
            }
            //开始日期公司规定的正常早晨开始上班时间
            DateTime firstDayBeginWorkDate = DateTime.Parse(startDate.ToShortDateString() + " " + workingTime.StartTime.ToShortTimeString());
            //开始日期公司规定的中午休息开始时间
            DateTime firstDayLunchBreakStartDate = DateTime.Parse(startDate.ToShortDateString() + " " + workingTime.LunchBreakStartTime.ToShortTimeString());
            //开始日期公司规定的中午休息结束时间
            DateTime firstDayLunchBreakEndDate = DateTime.Parse(startDate.ToShortDateString() + " " + workingTime.LunchBreakEndTime.ToShortTimeString());
            //开始日期公司规定的下午下班时间
            DateTime firstDayEndWorkDate = DateTime.Parse(startDate.ToShortDateString() + " " + workingTime.EndTime.ToShortTimeString());

            //结束日期公司规定的上班时间字段
            DateTime lastDayBeginWorkDate = DateTime.Parse(endDate.ToShortDateString() + " " + workingTime.StartTime.ToShortTimeString());
            DateTime lastDayLunchBreakStartDate = DateTime.Parse(endDate.ToShortDateString() + " " + workingTime.LunchBreakStartTime.ToShortTimeString());
            DateTime lastDayLunchBreakEndDate = DateTime.Parse(endDate.ToShortDateString() + " " + workingTime.LunchBreakEndTime.ToShortTimeString());
            DateTime lastDayEndWorkDate = DateTime.Parse(endDate.ToShortDateString() + " " + workingTime.EndTime.ToShortTimeString());

            //第一种情况，当startDate 和 endDate 是同一天的时候，计算总计时间
            if (startDate.Year == endDate.Year && startDate.Month == endDate.Month && startDate.Day == endDate.Day)
            {
                //检查开始和结束时间是否在正常工作时间范围内
                if(startDate > firstDayEndWorkDate || endDate < firstDayBeginWorkDate)
                {
                    throw new InvalidOperationException(@"开始时间和结束时间应在正常的工作时间范围内");
                }

                if(startDate < firstDayBeginWorkDate)
                { 
                    //如果开始时间早于公司规定的正常早晨上班时间，设置开始时间为早晨上班时间
                    startDate = firstDayBeginWorkDate;
                }
                if(endDate > firstDayEndWorkDate)
                {
                    //如果结束时间晚于公司规定的下午下班时间，设置结束时间为下午下班时间
                    endDate = firstDayEndWorkDate;
                }

                if((startDate >= firstDayLunchBreakStartDate && startDate <= firstDayLunchBreakEndDate) && (endDate >= firstDayLunchBreakStartDate && endDate <= firstDayLunchBreakEndDate))
                {
                    //如果startDate开始时间和endDate结束时间同时位于中午休息时间内，抛出异常
                    throw new InvalidOperationException(@"开始和结束时间不能同时位于中午休息时间内");
                }
                if((startDate >= firstDayBeginWorkDate && startDate <= firstDayLunchBreakStartDate) && (endDate >= firstDayLunchBreakEndDate && endDate <= firstDayEndWorkDate))
                {
                    //开始日期位于上午正常工作时间段，并且结束日期位于下午正常工作时间段，排除中午休息时间后，返回取整的上限值
                    totalMinutes = (endDate - startDate).TotalMinutes - (firstDayLunchBreakEndDate - firstDayLunchBreakStartDate).TotalMinutes;
                    return (float)(Math.Ceiling(totalMinutes / 60) / workingHours);
                }
                if(((startDate >= firstDayBeginWorkDate && startDate <= firstDayLunchBreakStartDate) && (endDate >= firstDayBeginWorkDate && endDate <= firstDayLunchBreakStartDate)) || ((startDate >= firstDayLunchBreakEndDate && startDate <= firstDayEndWorkDate) && (endDate >= firstDayLunchBreakEndDate && endDate <= firstDayEndWorkDate)))
                {
                    //开始日期和结束日期同时位于上午正常工作时间段，或者开始日期和结束日期同时位于下午正常工作时间段
                    totalMinutes = (endDate - startDate).TotalMinutes;
                    return (float)(Math.Ceiling(totalMinutes / 60) / workingHours);
                }


                //test code
                bool r1 = startDate >= firstDayLunchBreakStartDate;
                bool r2 = startDate <= firstDayLunchBreakEndDate;
                bool r3 = endDate >= firstDayLunchBreakEndDate;
               // bool r4 = endDate>

                if((startDate >= firstDayLunchBreakStartDate && startDate <= firstDayLunchBreakEndDate) && (endDate >= firstDayLunchBreakEndDate && endDate <= firstDayEndWorkDate))
                {
                    //开始日期位于中午休息时间段，并且结束日期位于下午正常工作时间段
                    totalMinutes = (endDate - firstDayLunchBreakEndDate).TotalMinutes;
                    return (float)(Math.Ceiling(totalMinutes / 60) / workingHours);
                }
                if((startDate >= firstDayBeginWorkDate && startDate <= firstDayLunchBreakStartDate) && (endDate >= firstDayLunchBreakStartDate && endDate <= firstDayLunchBreakEndDate))
                {
                    //开始日期位于上午正常工作时间段，并且结束日期位于中午休息时间段
                    totalMinutes = (firstDayLunchBreakStartDate - startDate).TotalMinutes;
                    return (float)(Math.Ceiling(totalMinutes / 60) / workingHours);
                }
            }

            //第二种情况，开始时间startDate和结束时间endDate不是同一天，计算开始时间和结束时间两天的分钟时长统计
            if(startDate.ToShortDateString() != endDate.ToShortDateString())
            {
                //获取结束时间的那一天统计的分钟数
                double totalMinutesOfLastDay = CalculateTotalMinutesOfLastDay(endDate, lastDayBeginWorkDate, lastDayLunchBreakStartDate, lastDayLunchBreakEndDate, lastDayEndWorkDate);

                //第一天，开始时间在早晨上班时间之前，统计分钟数是第一天整天和最后一天分钟数之和
                if (startDate < firstDayBeginWorkDate)
                {
                    totalMinutes = (firstDayEndWorkDate - firstDayBeginWorkDate).TotalMinutes - (firstDayLunchBreakEndDate - firstDayLunchBreakStartDate).TotalMinutes + totalMinutesOfLastDay;
                }
                //第一天，开始时间在上午上班时间内，需要减去中午休息时间段
                if(startDate >= firstDayBeginWorkDate && startDate <= firstDayLunchBreakStartDate)
                {
                    totalMinutes = (firstDayEndWorkDate - startDate).TotalMinutes - (firstDayLunchBreakEndDate - firstDayLunchBreakStartDate).TotalMinutes + totalMinutesOfLastDay;
                }
                //第一天，开始时间在中午休息时间段内
                if(startDate >= firstDayLunchBreakStartDate && startDate <= firstDayLunchBreakEndDate)
                {
                    totalMinutes = (firstDayEndWorkDate - firstDayLunchBreakEndDate).TotalMinutes + totalMinutesOfLastDay;
                }
                //第一天，开始时间在下午上班时间段内
                if(startDate >= firstDayLunchBreakEndDate && startDate <= firstDayEndWorkDate)
                {
                    totalMinutes = (firstDayEndWorkDate - startDate).TotalMinutes + totalMinutesOfLastDay;
                }
                //第一天，开始时间晚于下午下班时间
                if(startDate >= firstDayEndWorkDate)
                {
                    totalMinutes = totalMinutesOfLastDay;
                }
                //两个日期的间隔天数初始化为-1
                int intervalDays = -1;
                DateTime initialStartDate = startDate;
                //循环统计开始时间startDate和结束时间endDate 中间间隔的天数，排除非工作日期
                do
                { 
                    initialStartDate = initialStartDate.AddDays(1);
                    if(IsWorkingDay(initialStartDate))
                    {
                        intervalDays++;
                    }

                } while (initialStartDate.ToShortDateString() != endDate.ToShortDateString());
                if(intervalDays > 0)
                {
                    totalDays = intervalDays;
                }
                totalDays = totalDays + (float)(Math.Ceiling(totalMinutes / 60) / workingHours);
            }
            
            
            return totalDays;
        }

        public static bool IsWorkingDay(DateTime dateTime)
        {
            if(dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            return true;
        }

        private static double CalculateTotalMinutesOfLastDay(DateTime endDate, DateTime lastDayBeginWorkDate, DateTime lastDayLunchBreakStartDate, DateTime lastDayLunchBreakEndDate, DateTime lastDayEndworkDate)
        {
            double totalMinutes = 0;
            //最后一天的开始时间等于公司规定的早晨上班时间
            DateTime startDate = lastDayBeginWorkDate;
            //如果结束日期小于早晨上班时间，返回0
            if(endDate <= lastDayBeginWorkDate)
            {
                return 0;
            }
            if(endDate >= lastDayBeginWorkDate && endDate <= lastDayLunchBreakStartDate)
            {
                //结束时间位于上午正常工作时间内
                totalMinutes = (endDate - startDate).TotalMinutes;
            }
            if(endDate >= lastDayLunchBreakStartDate && endDate <= lastDayLunchBreakEndDate)
            {
                //结束时间位于中午休息时间内
                totalMinutes = (lastDayLunchBreakStartDate - startDate).TotalMinutes;
            }
            if(endDate >= lastDayLunchBreakEndDate && endDate <= lastDayEndworkDate)
            {
                //结束时间位于下午正常工作时间内
                totalMinutes = (endDate - startDate).TotalMinutes - (lastDayLunchBreakEndDate - lastDayLunchBreakStartDate).TotalMinutes;
            }
            if(endDate >= lastDayEndworkDate)
            {
                //结束时间大于下午下班时间
                totalMinutes = (lastDayEndworkDate - startDate).TotalMinutes - (lastDayLunchBreakEndDate - lastDayLunchBreakStartDate).TotalMinutes;
            }

            return totalMinutes;


        }

        /// <summary>
        /// Determines whether approver can approve the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="approverEmail">The approver's email.</param>
        /// <returns>
        ///   <c>true</c> if this instance can approve the specified user; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanApprove(User user, string approverEmail)
        {
            string managerEmail = user.DirectManagerEmail;
            if (!String.IsNullOrEmpty(managerEmail))
            {
                if (managerEmail == approverEmail)
                {
                    return true;
                }

            }
            return false;

        }
    }
}