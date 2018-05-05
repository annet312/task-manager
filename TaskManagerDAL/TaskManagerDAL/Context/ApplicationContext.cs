using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Identity;


namespace TaskManagerDAL.Context
{
    public class TaskManagerContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<_Task> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TaskTemplate> TaskTemplates { get; set; }
       // public DbSet<UserProfile> UserProfiles { get; set; }

        //static TaskManagerContext()
        //{
        //    Database.SetInitializer<TaskManagerContext>(new IdentityDbInit());           
        //}
        public TaskManagerContext()
        {

        }
        public TaskManagerContext(string connectionString)
        : base(connectionString)
        {
           
        }
    }

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<TaskManagerContext>
    {
        protected override void Seed(TaskManagerContext context)
        {
            IdentityInitialSetup(context);
            TaskManagerInitialSetup(context);
            base.Seed(context);
        }
        public void IdentityInitialSetup(TaskManagerContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Creating 2 roles
            var role1 = new IdentityRole { Name = "Manager" };
            var role2 = new IdentityRole { Name = "Programmer" };

            // Adding them to manager of roles
            roleManager.Create(role1);
            roleManager.Create(role2);

            // Creating manager and programmer
            var manager = new ApplicationUser { Email = "Manager1_mail@gmail.com", UserName = "Manager1Name" };
            string password = "_Manager1_";

            var result = userManager.Create(manager, password);
            if (result.Succeeded)
            {
                // Adding role for him
                userManager.AddToRole(manager.Id, role1.Name);
                //userManager.AddToRole(manager.Id, role2.Name);
            }
            /// Add to table Person
            var person = new Person { Name = manager.UserName, EAdress = manager.Email, Role = role1.Name};
            var team = new Team { TeamName = "Team1" };
            context.Teams.Add(team);
            person.Team.Id = team.Id;
            context.People.Add(person);
        }
        public void TaskManagerInitialSetup(TaskManagerContext context)
        {
            context.Statuses.Add(new Status {Name = "New" });
            context.Statuses.Add(new Status { Name = "Underway" });
            context.Statuses.Add(new Status { Name = "Executed" });// was finished by Assignee
            context.Statuses.Add(new Status { Name = "Completed" });// was confirmed by Author
            context.Statuses.Add(new Status { Name = "Canceled" });
            context.Statuses.Add(new Status { Name = "Procrastinated" });
            context.Statuses.Add(new Status { Name = "Draft" });

            context.TaskTemplates.Add(new TaskTemplate { Name = "Default Template", TemplateId = null });
            context.TaskTemplates.Add(new TaskTemplate { Name = "Lead Up", TemplateId = 1 });
            context.TaskTemplates.Add(new TaskTemplate { Name = "Developing", TemplateId = 1 });
            context.TaskTemplates.Add(new TaskTemplate { Name = "Testing", TemplateId = 1 });
            context.TaskTemplates.Add(new TaskTemplate { Name = "Implementation", TemplateId = 1 });

        }
    }

}
