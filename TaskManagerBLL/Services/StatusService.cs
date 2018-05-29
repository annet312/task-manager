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
    public class StatusService : IStatusService
    {
        private readonly IUnitOfWork db;
        private readonly IMapper mapper;
        //private readonly ITaskService taskService;

        public StatusService(IUnitOfWork uow, IMapper mapper/*, ITaskService taskService*/)
        {
            db = uow;
            this.mapper = mapper;
            //this.taskService = taskService;
        }
        #region Status
        public IEnumerable<StatusBLL> GetAllStatuses()
        {
            var statuses = db.Statuses.GetAll();

            return mapper.Map<IEnumerable<Status>, IEnumerable<StatusBLL>>(statuses);
        }

        public IEnumerable<StatusBLL> GetStatuses()
        {
            return GetAllStatuses().Where(s => s.Name != "Closed");
        }

        public void SetNewStatus(int taskId, string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
            {
                throw new ArgumentNullException("Name of status is null or empty", "statusName");
            }
            Status status = db.Statuses.Find(s => (s.Name == statusName)).SingleOrDefault();

            _Task task = db.Tasks.Get(taskId);
            if (task == null)
            {
                throw new ArgumentException("Task wasn't found", "id");
            }
            task.Status = status ?? throw new ArgumentException("Status with this name wasn't found", "statusName");
            switch (statusName)
            {
                case "New":
                    {
                        task.DateStart = null;
                        task.Progress = 0;
                        break;
                    }
                case "In progress":
                    {
                        task.DateStart = DateTime.Now;
                        task.Progress = 0;
                        if (!task.ParentId.HasValue)
                        {
                            task.Progress = CalculateProgressOfSubtask(task.Id, task.Id, null);
                        }
                        break;
                    }
                case "Done":
                    {
                        task.Progress = 100;
                        if (!task.ParentId.HasValue)
                        {
                            IEnumerable<_Task> subtasks = db.Tasks.Find(t => t.ParentId == task.Id);
                            foreach (var subtask in subtasks)
                            {
                                subtask.StatusId = status.Id;
                                subtask.Progress = 100;
                                db.Tasks.Update(subtask);
                            }
                        }
                        break;
                    }
                case "Closed":
                    {
                        task.Progress = 100;
                        break;
                    }
                default:
                    break;
            }

            if (task.ParentId.HasValue)
            {
                int progress = CalculateProgressOfSubtask(task.ParentId.Value, task.Id, task.Progress);
                _Task mainTask = db.Tasks.Get(task.ParentId.Value);
                if (mainTask.Status.Name == "New")
                {
                    var underwayStatusList = new string[3] { "Executed", "Underway", "Completed" };
                    if (underwayStatusList.Contains(task.Status.Name))
                    {
                        mainTask.StatusId = db.Statuses.Find(s => (s.Name == "Underway")).Single().Id;
                    }
                }
                mainTask.Progress = progress;
                db.Tasks.Update(mainTask);
            }

            db.Tasks.Update(task);
            db.Save();
        }
        #endregion
        private int CalculateProgressOfSubtask(int mainTaskId, int changedSubtaskId, int? changedSubtaskProgress)
        {
            var MainTask = db.Tasks.Get(mainTaskId);

            IEnumerable<_Task> subtasks = db.Tasks.Find(s => (s.ParentId == mainTaskId) && (s.Id != changedSubtaskId));
                //taskService.GetSubtasksOfTask(mainTaskId).Where(s => s.Id != changedSubtaskId);
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
            sumProgress = sumProgress / num;
            return sumProgress;
        }
    }
}
