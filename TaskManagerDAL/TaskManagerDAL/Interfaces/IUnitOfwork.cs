using TaskManagerDAL.Entities;

namespace TaskManagerDAL.Interfaces
{
    public interface IUnitOfWork
    {

        IRepository<Person> People { get; }
        IRepository<Status> Statuses { get; }
        IRepository<_Task> Tasks { get; }
        IRepository<TaskTemplate> TaskTemplates { get; }
        IRepository<Team> Teams { get; }

        void Save();
    }
}
