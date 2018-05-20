using System;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Context;
using TaskManagerDAL.Entities;

namespace TaskManagerDAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private PersonRepository personRepository;
        private StatusRepository statusRepository;
        private TaskRepository taskRepository;
        private TaskTemplateRepository taskTemplateRepository;
        private TeamRepository teamRepository;

        private TaskManagerContext db;

        public UnitOfWork(string connectionString)
        {
            db = new TaskManagerContext(connectionString);
        }

        public IRepository<Person> People {
            get
            {
                if (personRepository == null)
                {
                    personRepository = new PersonRepository(db);
                }
                return personRepository;
            }
        }

        public IRepository<Status> Statuses
        {
            get
            {
                if (statusRepository == null)
                {
                    statusRepository = new StatusRepository(db);
                }
                return statusRepository;
            }
        }

        public IRepository<_Task> Tasks
        {
            get
            {
                if (taskRepository == null)
                {
                    taskRepository = new TaskRepository(db);
                }
                return taskRepository;
            }
        }

        public IRepository<TaskTemplate> TaskTemplates
        {
            get
            {
                if (taskTemplateRepository == null)
                {
                    taskTemplateRepository = new TaskTemplateRepository(db);
                }
                return taskTemplateRepository;
            }
        }

        public IRepository<Team> Teams
        {
            get
            {
                if (teamRepository == null)
                {
                    teamRepository = new TeamRepository(db);
                }
                return teamRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
