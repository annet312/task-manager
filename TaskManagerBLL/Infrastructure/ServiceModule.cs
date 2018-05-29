using System.Configuration;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Repositories;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Services;
using AutoMapper;
using Autofac;
using System.Reflection;
using System.Collections.Generic;

namespace TaskManagerBLL.Infrastructure
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().WithParameter("connectionString",connectionString);
            builder.RegisterType<TaskService>().As<ITaskService>();
            builder.RegisterType<StatusService>().As<IStatusService>();
            builder.RegisterType<PersonService>().As<IPersonService>();
            builder.RegisterType<FilterTaskService>().As<IFilterTaskService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile)).As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();
            
    }
}
