using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebProject.Domain;
using Microsoft.AspNet.Identity;

namespace WebProject.Infrastructure
{
    public class WebProjectDbContext : IdentityDbContext<User>
    {

        public WebProjectDbContext() : base("MSSQLServer") { }

    

        static WebProjectDbContext()
        {
            Database.SetInitializer<WebProjectDbContext>(new IdentityDbInit());
        }

        public static WebProjectDbContext Create()
        {
            return new WebProjectDbContext();
        }

        public virtual DbSet<AnnualLeaveStatistic> AnnualLeaveStatistics { get; set; }
        public virtual DbSet<CompanyVacationPolicy> CompanmyVacationPolicies { get; set; }
        public virtual DbSet<LeaveApplication> LeaveApplications { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<MeetingRoom> MeetingRooms { get; set; }
      //  public virtual DbSet<User> Users { get; set; }
      //  public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<WorkingTime> WorkingTimes { get; set; }
        public virtual DbSet<Attendee> Attendees { get; set; }

        //User Login information
        public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }
      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.HireDate).HasColumnType("datetime2").IsOptional();

            //Build relationship between MeetingRoom and Meetings
            modelBuilder.Entity<MeetingRoom>()
                .HasMany(m => m.Meetings)
                .WithRequired(m => m.MeetingRoom);
            //Build relationship between Meeting and User
            modelBuilder.Entity<Meeting>()
                .HasRequired(m => m.Owner)
                .WithMany();
            //Build relationship between Meeting and Attendee
            modelBuilder.Entity<Meeting>()
                .HasRequired(m => m.Attendee)
                .WithMany();

            modelBuilder.Entity<Attendee>()
                .HasMany(a => a.Users)
                .WithMany();
            //Build relationship between LeaveApplication and User
            modelBuilder.Entity<LeaveApplication>()
                .HasRequired(l => l.Initiator)
                .WithMany()
                .HasForeignKey(l => l.InitiatorFK);
            modelBuilder.Entity<LeaveApplication>()
                .HasRequired(l => l.Approver)
                .WithMany()
                .HasForeignKey(l => l.ApproverFK)
                .WillCascadeOnDelete(false);
           

            modelBuilder.Entity<AnnualLeaveStatistic>()
                .HasRequired(u => u.User)
                .WithMany();

        }



        /// <summary>
        /// Gets the user annuel leave by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Return null if it failed to find one</returns>
        //public UserAnnuelLeave GetUserAnnuelLeaveByUser(string userId)
        //{
        //    foreach(var userAnnuelLeave in UserAnnuelLeaves)
        //    {
        //        if(userAnnuelLeave.User.Id == userId)
        //        {
        //            return userAnnuelLeave;
        //        }
        //    }
        //    return null;
        //}
    }



    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<WebProjectDbContext>
    {
        protected override void Seed(WebProjectDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(WebProjectDbContext context)
        {
            WebProjectUserManager userMgr = new WebProjectUserManager(new UserStore<User>(context));
            WebProjectRoleManager roleMgr = new WebProjectRoleManager(new RoleStore<Role>(context));

            string roleName = "Admin";
            string managerRole = "Manager";
          
            string userName = "admin";
            string password = "123456";
            string email = "admin@admin.com";
            string fullName = "testadmin fullname";

            string userName1 = "manager";
            string password1 = "123456";
            string email1 = "manager@manager.com";
            string fullName1 = "testManager DisplayName";

            string userName2 = "test";
            string password2 = "123456";
            string email2 = "testuser@user.com";
            string fullName2 = "testuser fullname";

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new Role(roleName));
            }
            if (!roleMgr.RoleExists(managerRole))
            {
                roleMgr.Create(new Role(managerRole));
            }

            User user = userMgr.FindByName(userName);
            if (user == null)
            {
                User localUser = new User();
                localUser.UserName = userName;
                localUser.Email = email;
                localUser.FullName = fullName;
                localUser.DirectManagerEmail = email;

                userMgr.Create(localUser,password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }
            if (!userMgr.IsInRole(user.Id, managerRole))
            {
                userMgr.AddToRole(user.Id, managerRole);
            }

            User user1 = userMgr.FindByName(userName1);
            if (user1 == null)
            {
                User localUser = new User();
                localUser.UserName = userName1;
                localUser.Email = email1;
                localUser.FullName = fullName1;
                localUser.DirectManagerEmail = email;

                userMgr.Create(localUser, password1);
                user1 = userMgr.FindByName(userName1);
            }

            if (!userMgr.IsInRole(user1.Id, managerRole))
            {
                userMgr.AddToRole(user1.Id, managerRole);
            }

            User user2 = userMgr.FindByName(userName2);
            if (user2 == null)
            {
                User localUser = new User();
                localUser.UserName = userName2;
                localUser.Email = email2;
                localUser.FullName = fullName2;
                localUser.DirectManagerEmail = email1;

                userMgr.Create(localUser, password2);
               
            }

            //Add a new WorkingTime object to the new created Database
            WorkingTime workingTime = new WorkingTime();
            workingTime.StartTime = new DateTime(2000, 1, 1, 9, 0, 0);
            workingTime.EndTime = new DateTime(2000,1, 1, 18, 0, 0);
            workingTime.LunchBreakStartTime = new DateTime(2000, 1, 1, 12, 0, 0);
            workingTime.LunchBreakEndTime = new DateTime(2000, 1, 1, 13, 0, 0);
            context.WorkingTimes.Add(workingTime);
            context.SaveChanges();

            //Initialize a new CompanyVacationPolicy object
            CompanyVacationPolicy companyVacationPolicy = new CompanyVacationPolicy();
            companyVacationPolicy.AnnualLeaveInitialDays = 10;
            companyVacationPolicy.AnnualLeaveMaxDays = 20;
            context.CompanmyVacationPolicies.Add(companyVacationPolicy);
            context.SaveChanges();

            //Initialize MeetingRooms when it create the new database 
            MeetingRoom meetingRoom1 = new MeetingRoom();
            meetingRoom1.Id = "1";
            meetingRoom1.Name = "会议室一";
            meetingRoom1.IsHaveingProjector = true;
            meetingRoom1.IsHaveingProjector = true;
            meetingRoom1.IsHavingVideoDevice = true;
            meetingRoom1.IsHavingVoiceDevice = true;
            MeetingRoom meetingRoom2 = new MeetingRoom();
            meetingRoom2.Id = "2";
            meetingRoom2.Name = "会议室二";
            meetingRoom2.IsHaveingProjector = true;
            meetingRoom2.IsHaveingProjector = true;
            meetingRoom2.IsHavingVideoDevice = false;
            meetingRoom2.IsHavingVoiceDevice = true;
            MeetingRoom meetingRoom3 = new MeetingRoom();
            meetingRoom3.Id = "3";
            meetingRoom3.Name = "会议室三";
            meetingRoom3.IsHaveingProjector = true;
            meetingRoom3.IsHaveingProjector = true;
            meetingRoom3.IsHavingVideoDevice = true;
            meetingRoom3.IsHavingVoiceDevice = false;
            context.MeetingRooms.Add(meetingRoom1);
            context.MeetingRooms.Add(meetingRoom2);
            context.MeetingRooms.Add(meetingRoom3);
            context.SaveChanges();
        }
    }
}