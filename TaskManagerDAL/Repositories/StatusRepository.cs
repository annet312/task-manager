using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class StatusRepository : IRepository<Status>
    {
        private TaskManagerContext db;

        public StatusRepository(TaskManagerContext context)
        {
            db = context;
        }

        public IEnumerable<Status> GetAll()
        {
            return db.Statuses;
        }

        public Status Get(int id)
        {
            return db.Statuses.Find(id);
        }

        public IEnumerable<Status> Find(Func<Status, Boolean> predicate)
        {
            return db.Statuses.Where(predicate)/*.ToList()*/;
        }

        public void Create(Status status)
        {
            db.Statuses.Add(status);
        }

        public void Update(Status status)
        {
            db.Entry(status).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Status status = db.Statuses.Find(id);
            if (status != null)
            {
                db.Statuses.Remove(status);
            }
        }
    }
}
