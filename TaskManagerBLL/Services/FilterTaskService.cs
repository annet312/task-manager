using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Interfaces;

namespace TaskManagerBLL.Services
{
    public class FilterTaskService : IFilterTaskService
    {
        private IUnitOfWork db { get; set; }

        public FilterTaskService(IUnitOfWork uow)
        {
            db = uow;
        }

        #region FilterTask
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

            IEnumerable<TaskBLL> resulttasks = Mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
            return resulttasks;
        }

        public IEnumerable<TaskBLL> GetOverDueTasks(int teamId)
        {
            var manager = db.Teams.Get(teamId);
            var tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) && (t.ParentId == null)
                                        && ((t.DueDate < DateTime.Now) || (t.DueDate == null))));

            return Mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

        public IEnumerable<TaskBLL> GetCompleteTasks(int teamId)
        {
            var manager = db.Teams.Get(teamId);
            var tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) && (t.ParentId == null)
                                        && (t.Status.Name == "Closed")));

            return Mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

        public IEnumerable<TaskBLL> GetTaskOfAssignee(string id)
        {
            IEnumerable<_Task> tasks = db.Tasks.Find(t => ((t.Assignee.UserId == id) && (t.ParentId == null)))
                                               .OrderByDescending(t => t.Progress);
            IEnumerable<TaskBLL> result = Mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);

            return result;
        }
        #endregion
    }
}
