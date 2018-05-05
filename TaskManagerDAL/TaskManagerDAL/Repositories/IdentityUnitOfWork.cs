using System;
using System.Threading.Tasks;
using TaskManagerDAL.Identity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TaskManagerDAL.Repositories
{
    public class IdentityUnitOfWork : IIdentityUnitOfWork
    {
        private TaskManagerContext db;

        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;
        private IPersonManager personManager;

        public IdentityUnitOfWork(string connectionString)
        {
            db = new TaskManagerContext(connectionString);
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            personManager = new PersonManager(db);
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager;
            }
        }

        public IPersonManager RersonManager
        {
            get
            {
                return personManager;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return roleManager;
            }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager.Dispose();
                    roleManager.Dispose();
                    personManager.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
