using Moveax.Mvc.ErrorHandler;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
