using AutoMapper;
using Moveax.Mvc.ErrorHandler;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;
using TaskMng.Models;

namespace TaskMng
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.Initialize(config =>
            {
                config.CreateMap<TaskBLL, TaskView>();
                config.CreateMap<TeamBLL, TeamView>();
                config.CreateMap<StatusBLL, StatusView>();
                config.CreateMap<PersonBLL, PersonView>();

                config.CreateMap<Team, TeamBLL>();
                config.CreateMap<_Task, TaskBLL>();
                config.CreateMap<Status, StatusBLL>();
                config.CreateMap<TaskTemplate, TaskTemplateBLL>();
                config.CreateMap<Person, PersonBLL>();
            });
        }

        protected void Application_Error(object sender, System.EventArgs e)
        {
            var errorHandler = new MvcApplicationErrorHandler(application: this, exception: this.Server.GetLastError())
            {
                EnableHttpReturnCodes = false,
                PassThroughHttp401 = false
            };

            errorHandler.Execute();
        }
    }
}
