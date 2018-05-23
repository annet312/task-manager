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
    public class StatusService : IStatusService
    {
        private readonly IUnitOfWork db;
        private readonly ISubtaskService subtaskService;
        private IMapper mapper { get; set; }

        public StatusService(IUnitOfWork uow, ISubtaskService subtaskService)
        {
            db = uow;
            this.subtaskService = subtaskService;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Person, PersonBLL>();
                cfg.CreateMap<Status, StatusBLL>();
                cfg.CreateMap<TaskTemplate, TaskTemplateBLL>();
                cfg.CreateMap<Team, TeamBLL>();
                cfg.CreateMap<Task, TaskBLL>();
            });

            mapper = config.CreateMapper();
        }
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
                            task.Progress = subtaskService.CalculateProgressOfSubtask(task.Id, 0);
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
                int progress = subtaskService.CalculateProgressOfSubtask(task.ParentId.Value, task.Id, task.Progress);
                _Task mainTask = db.Tasks.Get(task.ParentId.Value);
                if (mainTask.Status.Name == "New")
                {
                    var underwayStatusList = new string[3] { "Done", "In progress", "Closed" };
                    if (underwayStatusList.Contains(task.Status.Name))
                    {
                        mainTask.StatusId = db.Statuses.Find(s => (s.Name == "In progress")).Single().Id;
                    }
                }
                mainTask.Progress = progress;
                db.Tasks.Update(mainTask);
            }

            db.Tasks.Update(task);
            db.Save();
        }
    }
}
