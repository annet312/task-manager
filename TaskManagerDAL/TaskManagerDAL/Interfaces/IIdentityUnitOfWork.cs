using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Threading.Tasks;
using TaskManagerDAL.Identity;

namespace TaskManagerDAL.Interfaces
{
    public interface IIdentityUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        IPersonManager RersonManager { get; }
        ApplicationRoleManager RoleManager { get; }
        Task SaveAsync();
    }
}