using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Domain
{
    public class UserLoginLog
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime LoginDateTime { get; set; }
        public string RemoteIP { get; set; }
    }
}