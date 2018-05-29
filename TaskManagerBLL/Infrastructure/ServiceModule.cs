using System.Configuration;
using Ninject.Modules;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Repositories;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Services;
using AutoMapper;

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
            var mapperConfiguration = CreateConfiguration();
            Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();

            Bind<IMapper>().ToMethod(ctx =>
                        new Mapper(mapperConfiguration));
        }
        private MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Add all profiles in current assembly
                cfg.AddProfile<PersonProfile>();
                cfg.AddProfile<StatusProfile>();
                cfg.AddProfile<TaskProfile>();
                cfg.AddProfile<TeamProfile>();
                cfg.AddProfile<TaskTemplateProfile>();
            });

            return config;
        }
    }
}
