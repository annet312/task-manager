using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerUsersBLL.Interfaces
{
    public interface ICreateService
    {
        IUserService CreateUserService(string connection);
    }
}
