using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class TaskTemplateRepository : IRepository<TaskTemplate>
    {
        private TaskManagerContext db { get; set; }

        public TaskTemplateRepository(TaskManagerContext context)
        {
            db = context;
        }

        public IEnumerable<TaskTemplate> GetAll()
        {
            return db.TaskTemplates.ToList();
        }

        public TaskTemplate Get(int id)
        {
            return db.TaskTemplates.Find(id);
        }

        public IEnumerable<TaskTemplate> Find(Func<TaskTemplate, Boolean> predicate)
        {
            return db.TaskTemplates.Where(predicate).ToList();
        }

        public void Create(TaskTemplate taskTemplate)
        {
            db.TaskTemplates.Add(taskTemplate);
        }

        public void Update(TaskTemplate taskTemplate)
        {
            db.Entry(taskTemplate).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            TaskTemplate taskTemplate = db.TaskTemplates.Find(id);
            if (taskTemplate != null)
            {
                db.TaskTemplates.Remove(taskTemplate);
            }
        }
    }
}
