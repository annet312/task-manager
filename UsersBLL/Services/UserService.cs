using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TaskManagerUsersBLL.Interfaces;
using TaskManagerDAL.Interfaces;
using TaskManagerUsersBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerUsersBLL.Infrastructure;
using System.Security.Claims;
using System.Linq;

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
                //!!!!!TeamName надо находить
                Database.RersonManager.Create(person, userBll.TeamName);
                await Database.SaveAsync();

                return new IdentityOperation(true, "Sign up is success", "");

            }
            else
            {
                return new IdentityOperation(false, "User with this name is already exist", "Email");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserBLL userBll)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            var user = await Database.UserManager.FindAsync(userBll.UserName, userBll.Password);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        public async Task SetInitialData(UserBLL adminBll/*, List<string> roles*/)
        {
            //foreach (string roleName in roles)
            //{
            //    var role = await Database.RoleManager.FindByNameAsync(roleName);
            //    if (role == null)
            //    {
            //        role = new ApplicationRole { Name = roleName };
            //        await Database.RoleManager.CreateAsync(role);
            //    }
            //}

            await Create(adminBll);
        }


        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
