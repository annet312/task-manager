using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Interfaces;

namespace TaskManagerBLL.Services
{
    public class FilterTasksService : IFilterTasks
    {
        private IUnitOfWork db { get; set; }

        private IMapper mapper { get; set; }

        public FilterTasksService(IUnitOfWork uow)
        {
            db = uow;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Person, PersonBLL>();
                cfg.CreateMap<Status, StatusBLL>();
                cfg.CreateMap<TaskTemplate, TaskTemplateBLL>();
                cfg.CreateMap<Team, TeamBLL>();
                cfg.CreateMap<Task, TaskBLL>();
            });

            mapper = config.CreateMapper();
        }

        public IEnumerable<TaskBLL> GetTasksOfTeam(string managerId)
        {
            var manager = db.People.Find(p => p.UserId == managerId).SingleOrDefault();
            if (manager == null)
            {
                throw new ArgumentException("Manager is not found", "managerId");
            }
            IEnumerable<_Task> tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) &&
                                            (t.ParentId == null) &&
                                            (t.Assignee.Id != manager.Id))).OrderBy(tsk => tsk.Assignee.Name).ToList();

            IEnumerable<TaskBLL> resulttasks = mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
            return resulttasks;
        }

        public IEnumerable<TaskBLL> GetOverDueTasks(int teamId)
        {
            var manager = db.Teams.Get(teamId);
            var tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) && (t.ParentId == null)
                                        && ((t.DueDate < DateTime.Now) || (t.DueDate == null))));

            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

        public IEnumerable<TaskBLL> GetCompleteTasks(int teamId)
        {
            var manager = db.Teams.Get(teamId);
            var tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) && (t.ParentId == null)
                                        && (t.Status.Name == "Closed")));

            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

        public IEnumerable<TaskBLL> GetTaskOfAssignee(string id)
        {
            IEnumerable<_Task> tasks = db.Tasks.Find(t => ((t.Assignee.UserId == id) && (t.ParentId == null)))
                                               .OrderByDescending(t => t.Progress);
            IEnumerable<TaskBLL> result = mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);

            return result;
        }
    }
}
