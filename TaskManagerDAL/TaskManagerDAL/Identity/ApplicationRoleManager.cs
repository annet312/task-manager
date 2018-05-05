using Microsoft.AspNet.Identity.EntityFramework;
using TaskManagerDAL.Entities;

namespace TaskManagerDAL.Identity
{
    public class ApplicationRoleManager : Microsoft.AspNet.Identity.RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(RoleStore<ApplicationRole> store)
                    : base(store)
        { }
    }
}
