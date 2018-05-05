using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagerUsersBLL.Models;
using TaskManagerUsersBLL.Infrastructure;

namespace TaskManagerUsersBLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<IdentityOperation> Create(UserBLL userBll);
        Task<ClaimsIdentity> Authenticate(UserBLL userBll);
        Task SetInitialData(UserBLL adminBll/*, List<string> roles*/);
    }
}
