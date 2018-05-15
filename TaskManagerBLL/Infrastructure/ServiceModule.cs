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
       
        public override void Load()
        {
            //"DefaultConnection";TODO
            connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Anna\\Documents\\LabsEpam\\task-manager\\TaskMng\\App_Data\\TaskMng.mdf; Integrated Security = True";

            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument(connectionString);
            Bind<ITaskService>().To<TaskService>().InSingletonScope();
            Bind<IPersonService>().To<PersonService>().InSingletonScope();

        }
    }
}
