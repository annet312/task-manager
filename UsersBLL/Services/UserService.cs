using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TaskManagerUsersBLL.Interfaces;
using TaskManagerDAL.Interfaces;
using TaskManagerUsersBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerUsersBLL.Infrastructure;
using System.Security.Claims;
using System.Linq;
using System;

namespace TaskManagerUsersBLL.Services
{
    public class UserService : IUserService
    {
        IIdentityUnitOfWork Database { get; set; }

        public UserService(IIdentityUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<IdentityOperation> Create(UserBLL userBll)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userBll.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userBll.Email, UserName = userBll.UserName };

                var result = await Database.UserManager.CreateAsync(user, userBll.Password);
                if (result.Errors.Count() > 0)
                {
                    return new IdentityOperation(false, result.Errors.FirstOrDefault(), "");
                }
                await Database.UserManager.AddToRoleAsync(user.Id, userBll.Role);

                var person = new Person { UserId = user.Id, Email = userBll.Email, Name = userBll.UserName , Role = userBll.Role};
                try
                {
                    if (userBll.Role == "Manager")
                    {
                        try
                        {
                            Database.RersonManager.Create(person, userBll.TeamName);
                        }
                        catch (ArgumentException e)
                        {
                            return new IdentityOperation(false, e.Message, e.ParamName);
                        }
                    }
                    else
                    {
                        Database.RersonManager.Create(person);
                    }
                }
                catch (ArgumentException e)
                {
                    return new IdentityOperation(false, e.Message, e.ParamName);
                }
                await Database.SaveAsync();

                return new IdentityOperation(true, "Sign up is success", "");

            }
            else
            {
                return new IdentityOperation(false, "User with this email is already exist", "Email");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserBLL userBll)
        {
            ClaimsIdentity claim = null;
            // Find  user
            var user = await Database.UserManager.FindAsync(userBll.UserName, userBll.Password);
            // authenticate him and return ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
