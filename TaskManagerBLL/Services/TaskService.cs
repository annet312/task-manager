using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerBLL.Interfaces;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerBLL.Models;
using AutoMapper;

namespace TaskManagerBLL.Services
{
    public class TaskService : ITaskService
    {
        private IUnitOfWork db { get; set; }
        private IMapper mapper { get; set; }
        public TaskService(IUnitOfWork uow)
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
        public TaskBLL GetTask(int id)
        {
            var result = db.Tasks.Get(id);
            return mapper.Map<_Task, TaskBLL>(result);
        }
        public IEnumerable<StatusBLL> GetAllStatuses()
        {
            var statuses = db.Statuses.GetAll();
            if (!statuses.Any())
            {
                throw new Exception("test"); //for debug
            }
            return mapper.Map<IEnumerable<Status>, IEnumerable<StatusBLL>>(statuses);
        }

        public IEnumerable<TaskTemplateBLL> GetAllTemplates()
        {
            var result = db.TaskTemplates.Find(t => (t.TemplateId == null));
            return mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
        }

        public IEnumerable<TaskTemplateBLL> GetSubtasksOfTemplate(int tempplateId)
        {
            var result = db.TaskTemplates.Find(t => (t.TemplateId == tempplateId));
            return mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
        }

        public void DeleteTask(int taskId)
        {
            var task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            if (task.ParentId == null)
            {
                var subtasksForDelete = GetSubtasksOfTask(task.Id);
                foreach (var subtask in subtasksForDelete)
                {
                    db.Tasks.Delete(subtask.Id);
                }
            }
            db.Tasks.Delete(task.Id);
            db.Save();
        }

        public void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true)
        {
            var task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            var status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).FirstOrDefault());
            var newSubtask = new TaskBLL { ParentId = task.Id,
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
                db.Save();//??
            }
        }
        public void AddSubtasksFromTemplate(int taskId, int templateId)
        {
            var subtaskNames = GetSubtasksOfTemplate(templateId);
            foreach (var subtaskName in subtaskNames)
            {
                var subtask = new TaskBLL
                {
                    Name = subtaskName.Name,
                    ParentId = taskId,
                    ETA = null,
                    DueDate = null,
                    Comment = null
                };
                AddSubtask(subtask, taskId, false);
            }
            db.Save();
        }
        public void CreateTask(TaskBLL task, string authorId,string assigneeId )
        {
            PersonBLL author = mapper.Map<Person,PersonBLL>(db.People.Find(p => p.UserId == authorId).Single());
            
            PersonBLL assigneeBLL;
            if ((assigneeId == string.Empty) || (assigneeId == null))
            {
                assigneeBLL = author;
            }
            else
            {
                Person assignee = db.People.Find(p => p.UserId == assigneeId).Single();
                assigneeBLL = mapper.Map<Person, PersonBLL>(assignee);
            }
            var status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).Single());
            var newTask = new TaskBLL
            {
                ParentId = null,
                Author = author,
                Name = task.Name,
                Assignee = assigneeBLL,
                Status = status,
                Progress = 0,
                DateStart = null,
                ETA = task.ETA,
                DueDate = task.DueDate,
                Comment = task.Comment
            };
            db.Tasks.Create(mapper.Map<TaskBLL, _Task>(newTask));
            db.Save();
        }
        public void SaveChangeTask(TaskBLL task)
        {//!!!!TODO UPDATE except delete/add
            db.Tasks.Delete(task.Id);
            var oldId = task.Id;
            db.Tasks.Create(mapper.Map<TaskBLL, _Task>(task));
            if (task.ParentId == null)
            {
                var subtasks = db.Tasks.Find(t => (t.ParentId == oldId));
                foreach (var subtask in subtasks)
                {
                    var oldSubtask = db.Tasks.Get(subtask.Id);
                    db.Tasks.Delete(subtask.Id);
                    oldSubtask.ParentId = task.Id;
                    db.Tasks.Create(oldSubtask);
                }
            }
            db.Save();
        }
        public IEnumerable<TaskBLL> GetSubtasksOfTask(int Id)
        {
            var subtasks = db.Tasks.Find(t => (t.ParentId == Id));
            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(subtasks);
        }
        private int CalculateProgressofSubTask(int Id)
        {
            var MainTask = db.Tasks.Get(Id);
            var subtasks = GetSubtasksOfTask(Id);
            var sumProgress = 0;
            var num = 0;
            if (MainTask.Progress.HasValue)
            {
                sumProgress = MainTask.Progress.Value;
            }
            num++;

            foreach (var subtask in subtasks)
            {
                if (subtask.Progress.HasValue)
                {
                    sumProgress += subtask.Progress.Value;
                }
                num++;
            }
            sumProgress = sumProgress / num;
            return sumProgress;
        }
        public void SetNewStatus(int statusId, TaskBLL task)
        {
            var status = db.Statuses.Get(statusId);
            var OldTask = db.Tasks.Get(task.Id);
            OldTask.Status = status;

            switch (status.Name)
            {
                case "Underway":
                    {
                        OldTask.DateStart = DateTime.Now;
                        OldTask.Progress = 0;
                        break;
                    }
                case "Execute":
                    {
                        OldTask.Progress = 100;
                        OldTask.DueDate = DateTime.Now;
                        break;
                    }
                case "Complete":
                    {
                        OldTask.Progress = 100;
                        OldTask.DueDate = DateTime.Now;
                        break;
                    }
                default:
                    {
                        OldTask.Progress = 0;
                    }
                    break;
            }
            if (OldTask.ParentId.HasValue)
            {
                var progress = CalculateProgressofSubTask(OldTask.ParentId.Value);
                var taskForChange = db.Tasks.Get(OldTask.ParentId.Value);
                db.Tasks.Delete(taskForChange.Id);
                db.Tasks.Create(taskForChange);
            }
            db.Tasks.Create(OldTask);
        }

        public IEnumerable<TaskBLL> GetTasksOfTeam(string managerId)
        {
            var manager = db.People.Find(p => p.UserId == managerId).SingleOrDefault();//TODO validation
            var tasks = db.Tasks.Find(t => ((t.Author.Id == manager.Id) && (t.ParentId == null)));
            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
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
                                        && (t.Status.Name == "Complete")));
            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

        public IEnumerable<TaskBLL> GetTaskOfAssignee(string id)
        {
            var tasks = db.Tasks.Find(t => ((t.Assignee.UserId == id) && (t.ParentId == null)));
            return mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(tasks);
        }

    }
}
