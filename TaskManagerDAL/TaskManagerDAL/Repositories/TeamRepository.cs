using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class TeamRepository : IRepository<Team>
    {
        private TaskManagerContext db;
        public TeamRepository(TaskManagerContext context)
        {
            db = context;
        }
        public IEnumerable<Team> GetAll()
        {
            return db.Teams.ToList();
        }
        public Team Get(int id)
        {
            return db.Teams.Find(id);
        }
        public IEnumerable<Team> Find(Func<Team, Boolean> predicate)
        {
            return db.Teams.Where(predicate).ToList();
        }
        public void Create(Team team)
        {
            db.Teams.Add(team);
        }
        public void Update(Team team)
        {
            db.Entry(team).State = EntityState.Modified;
        }
        public void Delete(int id)
        {
            Team team = db.Teams.Find(id);
            if (team != null)
            {
                db.Teams.Remove(team);
            }
        }
    }
}
