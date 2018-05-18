using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Repositories;
using TaskManagerUsersBLL.Interfaces;
using TaskManagerUsersBLL.Services;

namespace UsersBLL.Infrastructure
{
    public class IdentityServiceModule : NinjectModule
    {
        private string connectionString;

        public override void Load()
        {
            //"DefaultConnection";TODO
            connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Anna\\Documents\\LabsEpam\\task-manager\\TaskMng\\App_Data\\TaskMng.mdf; Integrated Security = True";

            Bind<IIdentityUnitOfWork>().To<IdentityUnitOfWork>().WithConstructorArgument(connectionString);
            Bind<ICreateService>().To<CreateService>().InSingletonScope();
            Bind<IUserService>().To<UserService>().InSingletonScope();
        }
    }
}
