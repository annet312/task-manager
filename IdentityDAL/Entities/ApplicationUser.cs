using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityDAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual UserProfile UserProfile { get; set; }
    }
}
