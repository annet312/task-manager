using AutoMapper;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System;
using System.Web;
using TaskManagerBLL.Infrastructure;
using TaskManagerBLL.Models;
using TaskMng.App_Start;
using TaskMng.Models;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace TaskMng.App_Start
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
            Mapper.Initialize(config =>
            {
                config.CreateMap<TaskBLL, TaskView>();
                config.CreateMap<TeamBLL, TeamView>();
                config.CreateMap<StatusBLL, StatusView>();
                config.CreateMap<PersonBLL, PersonView>();
            });
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            return Container;
        }

        public static T GetConcreteInstance<T>()
        {
            object instance = Container.TryGet<T>();
            if (instance != null)
                return (T)instance;
            throw new InvalidOperationException(string.Format("Unable to create an instance of {0}", typeof(T).FullName));
        }

        private static IKernel _container;
        public static IKernel Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new StandardKernel();
                    _container.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                    _container.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                    RegisterServices(_container);
                }
                return _container;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new INinjectModule[]
                        {
                            new ServiceModule()
                        });
        }
    }
}
