using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Interfaces;

namespace TaskManagerBLL.Services
{
    public class SubtaskService : ISubtaskService
    {
        private IUnitOfWork db { get; set; }

        private IMapper mapper { get; set; }

        public SubtaskService(IUnitOfWork uow)
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

        #region Subtask
        public IEnumerable<TaskBLL> GetSubtasksOfTask(int parentId)
        {
            var subtasks = db.Tasks.Find(t => (t.ParentId == parentId));
            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(subtasks);
        }

        public void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true)
        {
            var task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            if (task == null)
            {
                throw new ArgumentException("Parent task wasn't found", "taskId");
            }

            var status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).Single());

            var newSubtask = new TaskBLL
            {
                ParentId = task.Id,
                Author = task.Author,
                Name = subtask.Name,
                Assignee = task.Assignee,
                Status = status,
                Progress = 0,
                DateStart = null,
                ETA = subtask.ETA,
                DueDate = subtask.DueDate,
                Comment = subtask.Comment
            };
            db.Tasks.Create(mapper.Map<TaskBLL, _Task>(newSubtask));

            if (forceToSave)
            {
                _Task parentTask = db.Tasks.Get(taskId);
                parentTask.Progress = CalculateProgressOfSubtask(taskId, 1);
                db.Tasks.Update(parentTask);
                db.Save();
            }
        }

        public int CalculateProgressOfSubtask(int mainTaskId, int numAddedSubtasks)
        {
            var MainTask = db.Tasks.Get(mainTaskId);
            var subtasks = GetSubtasksOfTask(mainTaskId);
            int sumProgress = 0;
            int num = numAddedSubtasks;

            foreach (var subtask in subtasks)
            {
                if (subtask.Progress.HasValue)
                {
                    sumProgress += subtask.Progress.Value;
                }
                num++;
            }
            if (MainTask.Status.Name == "Closed")
            {
                return 100;
            }
            sumProgress = (num != 0) ? (sumProgress / num) : 0;
            return sumProgress;
        }

        public int CalculateProgressOfSubtask(int mainTaskId, int changedSubtaskId, int? changedSubtaskProgress)
        {
            var MainTask = db.Tasks.Get(mainTaskId);

            var subtasks = GetSubtasksOfTask(mainTaskId).Where(s => s.Id != changedSubtaskId);
            int sumProgress = changedSubtaskProgress ?? 0;
            int num = 1;

            foreach (var subtask in subtasks)
            {
                if (subtask.Progress.HasValue)
                {
                    sumProgress += subtask.Progress.Value;
                }
                num++;
            }

            if (MainTask.Status.Name == "Closed")
            {
                return 100;
            }
            sumProgress = (num != 0) ? (sumProgress / num) : 0; ;
            return sumProgress;
        }

        public int CalculateProgressOfSubtask(int mainTaskId, int deletedSubtaskId, bool deleting)
        {
            var MainTask = db.Tasks.Get(mainTaskId);
            IEnumerable<TaskBLL> subtasks = GetSubtasksOfTask(mainTaskId);
            var subtasksWithoutDeleted = subtasks.Where(s => (s.Id != deletedSubtaskId));
            int sumProgress = 0;
            int num = 0;

            foreach (var subtask in subtasksWithoutDeleted)
            {
                if (subtask.Progress.HasValue)
                {
                    sumProgress += subtask.Progress.Value;
                }
                num++;
            }
            if (num == 0)
            {
                if (MainTask.Status.Name == "Closed")
                {
                    return 100;
                }
                else
                {
                    return 0;
                }
            }

            sumProgress = (sumProgress / num);
            return sumProgress;
        }

        public void ChangeAssigneeOfSubtasks(int parentId, int assigneeId)
        {
            Person assignee;
            assignee = db.People.Get(assigneeId);

            IEnumerable<_Task> subtasks = db.Tasks.Find(t => (t.ParentId == parentId));

            foreach (var subtask in subtasks)
            {
                subtask.Assignee = assignee;
                db.Tasks.Update(subtask);
            }
        }
        #endregion

    }
}
