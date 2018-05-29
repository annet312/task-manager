using System.Configuration;
using Ninject.Modules;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Repositories;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Services;

namespace TaskManagerBLL.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument(connectionString);
            Bind<ITaskService>().To<TaskService>();//.InSingletonScope();
            Bind<IPersonService>().To<PersonService>().InSingletonScope();
            Bind<IEmailService>().To<EmailService>().InSingletonScope();
            Bind<IFilterTaskService>().To<FilterTaskService>();//.InSingletonScope();
            Bind<IStatusService>().To<StatusService>();//.InSingletonScope();

        }
    }
}
