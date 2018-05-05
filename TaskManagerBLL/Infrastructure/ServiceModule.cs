using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject.Modules;

using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Repositories;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Services;


namespace TaskManagerBLL.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private string connectionString;
        //public ServiceModule(string connection)
        //{
        //    connectionString = connection;
        //}
        public override void Load()
        {
            //"DefaultConnection";
            connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Anna\\Documents\\LabsEpam\\Project\\TaskMng\\App_Data\\TaskMng.mdf; Integrated Security = True";
            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument(connectionString);
            Bind<ITaskService>().To<TaskService>().InSingletonScope();
            Bind<IPersonService>().To<PersonService>().InSingletonScope();

        }
    }
}
