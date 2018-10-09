using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using WebProject.Domain;
using WebProject.Domain.Services;

namespace WebProject.Infrastructure
{
    public class WebProjectDbContext : DbContext
    {
        public WebProjectDbContext()
            : base("name=MSSQLServer")
        {
        }

        
        public virtual DbSet<AnnuelLeave> AnnuelLeaves { get; set; }
        public virtual DbSet<CompanyVacationPolicy> CompanmyVacationPolicies { get; set; }
        public virtual DbSet<LeaveApplication> LeaveApplications { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<MeetingRoom> MeetingRooms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<WorkingTime> WorkingTimes { get; set; }
        public virtual DbSet<Attendee> Attendees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
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
                .HasOptional(m => m.Attendee)
                .WithMany();
            //Build relationship between LeaveApplication and User
            modelBuilder.Entity<LeaveApplication>()
                .HasRequired(l => l.Initiator)
                .WithMany();
            //Build relationship between Approver and LeaveApplication
            modelBuilder.Entity<Approver>()
                .HasMany(a => a.ApprovingApplications)
                .WithRequired(l => l.Approver);
                
        }
    }
}