using System;
using System.Linq;
using System.Collections.Generic;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class PersonManager : IPersonManager
    {
        private TaskManagerContext db { get; set; }

        public PersonManager(TaskManagerContext context)
        {
            db = context;
        }

        public void Create(Person person, string teamNameStr)
        {
            var team = new Team { TeamName = teamNameStr };
            IEnumerable<Team> checkTeamExists = db.Teams.Where(t => (t.TeamName == teamNameStr)).ToList();

            if (checkTeamExists.Any())
            {
                throw new ArgumentException("This team already exists", teamNameStr);
            }

            db.Teams.Add(team);
            person.TeamId = team.Id;
            db.People.Add(person);
            db.SaveChanges();
        }

        public void Create(Person person)
        {
            db.People.Add(person);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
