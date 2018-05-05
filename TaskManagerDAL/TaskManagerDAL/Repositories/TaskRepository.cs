using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class TaskRepository : IRepository<_Task>
    {
        private TaskManagerContext dataBase;
        public TaskRepository(TaskManagerContext context)
        {
            dataBase = context;
        }
        public IEnumerable<_Task> GetAll()
        {
            return dataBase.Tasks.Include(p => p.Author).Include(p=>p.Assignee).Include(p => p.Status).ToList();
        }
        public _Task Get(int id)
        {
            return dataBase.Tasks.Find(id);
        }
        public IEnumerable<_Task> Find(Func<_Task, Boolean> predicate)
        {
            var tasks = dataBase.Tasks.Include(p => p.Author).Include(p => p.Assignee).Include(p => p.Status).Where(predicate).ToList();
            return tasks;
        }
        public void Create(_Task person)
        {
            dataBase.Tasks.Add(person);
        }
        public void Update(_Task task)
        {
            dataBase.Entry(task).State = EntityState.Modified;
        }
        public void Delete(int id)
        {
            _Task task = dataBase.Tasks.Find(id);
            if (task != null)
            {
                dataBase.Tasks.Remove(task);
            }
        }
    }
}
