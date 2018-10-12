using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Infrastructure;
using WebProject.Models;

namespace WebProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ManagementController : Controller
    {

        private ILogger _logger;
        private WebProjectDbContext _context;
       
        public ManagementController(ILogger logger, WebProjectDbContext webProjectDbContext)
        {
            this._logger = logger;
            this._context = webProjectDbContext;
           
        }
        // GET: Management
        public ActionResult AccessLog()
        {
            var logs = from l in _context.UserLoginLogs
                       orderby l.LoginDateTime descending
                       select new LoginLogViewModel
                       {
                           UserId = l.UserId,
                           UserName = l.UserName,
                           LoginDateTime = l.LoginDateTime,
                           RemoteIP = l.RemoteIP,
                       };
            ViewBag.AccessLogs = logs;
            return View();
        }

     
    }
}