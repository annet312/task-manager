
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Moveax.Mvc.ErrorHandler;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaskManagerBLL.Infrastructure;

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

            RegisterIOC();

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

        private void RegisterIOC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile)).As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            builder.RegisterModule(new ServiceModule());
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
           
        }

    }
}
