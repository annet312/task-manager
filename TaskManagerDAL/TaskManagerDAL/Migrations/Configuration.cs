namespace TaskManagerDAL.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TaskManagerDAL.Context;
    using TaskManagerDAL.Entities;
    using TaskManagerDAL.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<TaskManagerDAL.Context.TaskManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TaskManagerContext context)
        {
            //IdentityInitialSetup(context);
            TaskManagerInitialSetup(context);
            context.SaveChanges();
           
  //          base.Seed(context);
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
            var person = new Person { Name = manager.UserName, EAdress = manager.Email, Role = role1.Name };
            var team = new Team { TeamName = "Team1" };
            context.Teams.Add(team);
            person.Team.Id = team.Id;
            context.People.Add(person);
        }
        public void TaskManagerInitialSetup(TaskManagerContext context)
        {
            var statuses = new List<Status>
            {
                new Status { Name = "New" },
                new Status { Name = "Underway" },
                new Status { Name = "Executed" },
                new Status { Name = "Completed" },
                new Status { Name = "Canceled" },
                new Status { Name = "Postpone" },
                new Status { Name = "Draft" }
            };
            statuses.ForEach(status => context.Statuses.AddOrUpdate(s => s.Name, status));

            var templates = new List<TaskTemplate>
            {
                new TaskTemplate { Name = "Default Template", TemplateId = null },
                new TaskTemplate { Name = "Analysis", TemplateId = 1 },
                new TaskTemplate { Name = "Development", TemplateId = 1 },
                new TaskTemplate { Name = "Testing", TemplateId = 1 },
                new TaskTemplate { Name = "Deployment", TemplateId = 1 }
            };
            templates.ForEach(template => context.TaskTemplates.AddOrUpdate(s => s.Name, template));
        }
    }
}
