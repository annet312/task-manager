using TaskManagerUsersBLL.Interfaces;
using TaskManagerDAL.Repositories;

namespace TaskManagerUsersBLL.Services
{
    public class CreateService : ICreateService
    {
        public IUserService CreateUserService(string connection)
        {
            return new UserService(new IdentityUnitOfWork(connection));
        }
    }
}
