using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models.Menu
{
    public class Menu
    {
        public List<MenuInformation> GetUserRoleMenu()
        {
            MenuInformation UserInfo = new MenuInformation("UserInfo",@"用户信息", @"fa fa-fw fa-user");
            MenuInformation PersonInfo = new MenuInformation("PersonalInfo",@"个人信息", "Account", "GetPersonInformation", @"/Account/GetPersonInformation", @"fa fa-caret-right");
            MenuInformation ChangePersonInfo = new MenuInformation("ChangeInfo",@"修改个人信息", "Account", "ChangePersonInformation", @"/Account/ChangePersonInformation", @"fa fa-caret-right");
            UserInfo.SubMenu.Add(PersonInfo);
            UserInfo.SubMenu.Add(ChangePersonInfo);

            MenuInformation Meeting = new MenuInformation("ManageMeeting",@"会议管理", @"fa fa-fw fa-calendar");
            MenuInformation MyMeetings = new MenuInformation("MyMeetings",@"我的会议", "Meeting", "MyMeetings", @"/Meeting/MyMeetings", @"fa fa-caret-right");
            MenuInformation FirstMeetingRoom = new MenuInformation("FirstMeetingRoom",@"会议室一", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/1", @"fa fa-caret-right");
            MenuInformation SecondMeetingRoom = new MenuInformation("SecondMeetingRoom",@"会议室二", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/2", @"fa fa-caret-right");
            MenuInformation ThirdMeetingRoom = new MenuInformation("ThirdMeetingRoom",@"会议室三", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/3", @"fa fa-caret-right");
            Meeting.SubMenu.Add(MyMeetings);
            Meeting.SubMenu.Add(FirstMeetingRoom);
            Meeting.SubMenu.Add(SecondMeetingRoom);
            Meeting.SubMenu.Add(ThirdMeetingRoom);

            MenuInformation Leave = new MenuInformation("Leave",@"休假管理", @"fa fa-fw fa-tags");
            MenuInformation AnnualLeave = new MenuInformation("MyAnnualLeave",@"我的年假申请", "Leave", "MyAnnualLeave", @"/Leave/MyAnnualLeave", @"fa fa-caret-right");
            MenuInformation CreateAnnualLeave = new MenuInformation("CreateAnnualLeave",@"新建年假申请", "Leave", "AnnualLeaveApplication", @"/Leave/AnnualLeaveApplication", @"fa fa-caret-right");
            Leave.SubMenu.Add(AnnualLeave);
            Leave.SubMenu.Add(CreateAnnualLeave);

            MenuInformation Help = new MenuInformation("Help",@"帮助", @"fa fa-fw fa-question-circle");

            List<MenuInformation> returnList = new List<MenuInformation>();
            returnList.Add(UserInfo);
            returnList.Add(Meeting);
            returnList.Add(Leave);
            returnList.Add(Help);

            return returnList;
        }

        public List<MenuInformation> GetManagerMenu()
        {
            MenuInformation UserInfo = new MenuInformation("UserInfo", @"用户信息", @"fa fa-fw fa-user");
            MenuInformation PersonInfo = new MenuInformation("PersonalInfo", @"个人信息", "Account", "GetPersonInformation", @"/Account/GetPersonInformation", @"fa fa-caret-right");
            MenuInformation ChangePersonInfo = new MenuInformation("ChangeInfo", @"修改个人信息", "Account", "ChangePersonInformation", @"/Account/ChangePersonInformation", @"fa fa-caret-right");
            UserInfo.SubMenu.Add(PersonInfo);
            UserInfo.SubMenu.Add(ChangePersonInfo);

            MenuInformation Meeting = new MenuInformation("ManageMeeting", @"会议管理", @"fa fa-fw fa-calendar");
            MenuInformation MyMeetings = new MenuInformation("MyMeetings", @"我的会议", "Meeting", "MyMeetings", @"/Meeting/MyMeetings", @"fa fa-caret-right");
            MenuInformation FirstMeetingRoom = new MenuInformation("FirstMeetingRoom", @"会议室一", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/1", @"fa fa-caret-right");
            MenuInformation SecondMeetingRoom = new MenuInformation("SecondMeetingRoom", @"会议室二", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/2", @"fa fa-caret-right");
            MenuInformation ThirdMeetingRoom = new MenuInformation("ThirdMeetingRoom", @"会议室三", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/3", @"fa fa-caret-right");
            Meeting.SubMenu.Add(MyMeetings);
            Meeting.SubMenu.Add(FirstMeetingRoom);
            Meeting.SubMenu.Add(SecondMeetingRoom);
            Meeting.SubMenu.Add(ThirdMeetingRoom);

            MenuInformation Leave = new MenuInformation("Leave", @"休假管理", @"fa fa-fw fa-tags");
            MenuInformation AnnualLeave = new MenuInformation("MyAnnualLeave", @"我的年假申请", "Leave", "MyAnnualLeave", @"/Leave/MyAnnualLeave", @"fa fa-caret-right");
            MenuInformation CreateAnnualLeave = new MenuInformation("CreateAnnualLeave", @"新建年假申请", "Leave", "AnnualLeaveApplication", @"/Leave/AnnualLeaveApplication", @"fa fa-caret-right");
            MenuInformation MyApprovingAnnualLeave = new MenuInformation("ApprovingAnnualLeave",@"我的年假审批", "Leave", "MyApprovingAnnualLeave", @"/Leave/MyApprovingAnnualLeave", @"fa fa-caret-right");
            Leave.SubMenu.Add(AnnualLeave);
            Leave.SubMenu.Add(CreateAnnualLeave);
            Leave.SubMenu.Add(MyApprovingAnnualLeave);

            MenuInformation Help = new MenuInformation("Help",@"帮助", @"fa fa-fw fa-question-circle");

            List<MenuInformation> returnList = new List<MenuInformation>();
            returnList.Add(UserInfo);
            returnList.Add(Meeting);
            returnList.Add(Leave);
            returnList.Add(Help);

            return returnList;
        }

        public List<MenuInformation> GetAdminMenu()
        {
            MenuInformation UserInfo = new MenuInformation("UserInfo", @"用户信息", @"fa fa-fw fa-user");
            MenuInformation PersonInfo = new MenuInformation("PersonalInfo", @"个人信息", "Account", "GetPersonInformation", @"/Account/GetPersonInformation", @"fa fa-caret-right");
            MenuInformation ChangePersonInfo = new MenuInformation("ChangeInfo", @"修改个人信息", "Account", "ChangePersonInformation", @"/Account/ChangePersonInformation", @"fa fa-caret-right");
            UserInfo.SubMenu.Add(PersonInfo);
            UserInfo.SubMenu.Add(ChangePersonInfo);

            MenuInformation Meeting = new MenuInformation("ManageMeeting", @"会议管理", @"fa fa-fw fa-calendar");
            MenuInformation MyMeetings = new MenuInformation("MyMeetings", @"我的会议", "Meeting", "MyMeetings", @"/Meeting/MyMeetings", @"fa fa-caret-right");
            MenuInformation FirstMeetingRoom = new MenuInformation("FirstMeetingRoom", @"会议室一", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/1", @"fa fa-caret-right");
            MenuInformation SecondMeetingRoom = new MenuInformation("SecondMeetingRoom", @"会议室二", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/2", @"fa fa-caret-right");
            MenuInformation ThirdMeetingRoom = new MenuInformation("ThirdMeetingRoom", @"会议室三", "Meeting", "MeetingRoom", @"/Meeting/MeetingRoom/3", @"fa fa-caret-right");
            Meeting.SubMenu.Add(MyMeetings);
            Meeting.SubMenu.Add(FirstMeetingRoom);
            Meeting.SubMenu.Add(SecondMeetingRoom);
            Meeting.SubMenu.Add(ThirdMeetingRoom);

            MenuInformation Leave = new MenuInformation("Leave", @"休假管理", @"fa fa-fw fa-tags");
            MenuInformation AnnualLeave = new MenuInformation("MyAnnualLeave", @"我的年假申请", "Leave", "MyAnnualLeave", @"/Leave/MyAnnualLeave", @"fa fa-caret-right");
            MenuInformation CreateAnnualLeave = new MenuInformation("CreateAnnualLeave", @"新建年假申请", "Leave", "AnnualLeaveApplication", @"/Leave/AnnualLeaveApplication", @"fa fa-caret-right");
            MenuInformation MyApprovingAnnualLeave = new MenuInformation("ApprovingAnnualLeave", @"我的年假审批", "Leave", "MyApprovingAnnualLeave", @"/Leave/MyApprovingAnnualLeave", @"fa fa-caret-right");
            Leave.SubMenu.Add(AnnualLeave);
            Leave.SubMenu.Add(CreateAnnualLeave);
            Leave.SubMenu.Add(MyApprovingAnnualLeave);

            MenuInformation Account = new MenuInformation("ManageUser",@"用户管理", @"fa fa-fw fa-users");
            MenuInformation UserList = new MenuInformation("UserList",@"用户列表", "Account", "GetUsers", @"/Account/GetUsers", @"fa fa-caret-right");
            MenuInformation CreateUser = new MenuInformation("CreateUser",@"创建用户", "Account", "CreateUser", @"/Account/CreateUser", @"fa fa-caret-right");
            Account.SubMenu.Add(UserList);
            Account.SubMenu.Add(CreateUser);

            MenuInformation System = new MenuInformation("System",@"系统管理", @"fa fa-fw fa-ravelry");
            MenuInformation Log = new MenuInformation("Log",@"日志", "Management", "AccessLog", @"/Management/AccessLog", @"fa fa-caret-right");
            System.SubMenu.Add(Log);

            MenuInformation Help = new MenuInformation("Help",@"帮助", @"fa fa-fw fa-question-circle");

            List<MenuInformation> returnList = new List<MenuInformation>();
            returnList.Add(UserInfo);
            returnList.Add(Meeting);
            returnList.Add(Leave);
            returnList.Add(Account);
            returnList.Add(System);
            returnList.Add(Help);

            return returnList;
        }
    }
}