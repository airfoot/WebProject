using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using log4net.Config;
using Autofac;
using Autofac.Integration.Mvc;
using WebProject.Domain.Services;
using WebProject.Infrastructure;


namespace WebProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //configure Autofac
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<Logger>().As<ILogger>().InstancePerRequest();
            builder.RegisterType<WebProjectDbContext>().InstancePerRequest();
            builder.RegisterType<LeaveApplicationService>().As<ILeaveApplicationService>().InstancePerRequest();
            builder.RegisterType<MeetingService>().As<IMeetingService>().InstancePerRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(HttpContext.Current.Server.MapPath(@"~/Log/Log4Net.config")));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
