using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

using TaskManagerUsersBLL.Interfaces;
using TaskManagerUsersBLL.Services;

[assembly: OwinStartup(typeof(TaskMng.App_Start.Startup))]

namespace TaskMng.App_Start
{
    public class Startup
    {
        private ICreateService serviceCreator = new CreateService();

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IUserService>(CreateUserService);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }

        private IUserService CreateUserService()
        {
            return serviceCreator.CreateUserService("DefaultConnection");
        }
    }
}