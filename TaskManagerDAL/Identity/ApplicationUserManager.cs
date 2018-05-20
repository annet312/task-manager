
using Microsoft.AspNet.Identity;
using TaskManagerDAL.Entities;

namespace TaskManagerDAL.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
        {
        }
    }
}
