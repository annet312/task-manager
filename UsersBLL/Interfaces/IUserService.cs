using System;
using System.Security.Claims;
using System.Threading.Tasks;

using TaskManagerUsersBLL.Models;
using TaskManagerUsersBLL.Infrastructure;

namespace TaskManagerUsersBLL.Interfaces
{
    /// <summary>
    /// Service for working with users
    /// </summary>
    public interface IUserService : IDisposable
    {
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userBll">model of user</param>
        /// <returns>Information about result of creating</returns>
        Task<IdentityOperation> Create(UserBLL userBll);
        /// <summary>
        /// Authentication of user
        /// </summary>
        /// <param name="userBll">model of user</param>
        /// <returns></returns>
        Task<ClaimsIdentity> Authenticate(UserBLL userBll);
    }
}
