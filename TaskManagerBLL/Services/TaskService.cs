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
        public void CreateTask(TaskBLL task, string author,string assignee )
        {
            PersonBLL authorBLL = mapper.Map<Person,PersonBLL>(db.People.Find(p => p.Name == author).Single());
            
            PersonBLL assigneeBLL;
            if (string.IsNullOrEmpty(assignee))
            {
                assigneeBLL = authorBLL;
            }
            else
            {
                try
                {
                    assigneeBLL = mapper.Map<Person, PersonBLL>(db.People.Find(p => p.Name == assignee).Single());
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException( "Name of assignee not single in DataBase", e);
                }
            }
            StatusBLL status;
            try
            {
                status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).Single());
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException("Name of status 'new' not single in DataBase", e);
            }
            var newTask = new TaskBLL
            {
                ParentId = null,
                Author = authorBLL,
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
        public void SaveChangeTask(TaskBLL task, string assigneeName)
        {//!!!!TODO UPDATE except delete/add

            _Task taskForEdit = db.Tasks.Get(task.Id);

            if (taskForEdit != null)
            {
                if(taskForEdit.Name != task.Name)
                    taskForEdit.Name = task.Name;
                if(taskForEdit.ParentId != task.ParentId)
                    taskForEdit.ParentId = task.ParentId;
                if (assigneeName == null)
                {
                    throw new ArgumentNullException("assigneeName", "Cannot be null");
                }
                if (taskForEdit.Assignee.Name != assigneeName)
                {
                    Person assignee;
                    try
                    {
                        assignee = db.People.Find(p => (p.Name == assigneeName)).Single();
                    }
                    catch(InvalidOperationException e)
                    {
                        throw new InvalidOperationException("Assignee is not single in DataBase", e);
                    }
                    taskForEdit.Assignee = assignee;
                }
                if(taskForEdit.ETA != task.ETA)
                     taskForEdit.ETA = task.ETA;
                if(taskForEdit.DueDate != task.DueDate)
                    taskForEdit.DueDate = task.DueDate;
                if(taskForEdit.Comment != task.Comment)
                    taskForEdit.Comment = task.Comment;
                
                db.Tasks.Update(taskForEdit);
                db.Save();
            }
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
                sumProgress += MainTask.Progress.Value;
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
        public void SetNewStatus(int taskId, string statusName)
        {
            Status status;
            try
            {
                status = db.Statuses.Find(s => (s.Name == statusName)).Single();
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("Status name isn't single in DataBase",e);
            }
            _Task task = db.Tasks.Get(taskId);
            if (task == null)
            {
                throw new ArgumentException("Task wasn't found", "id");
            }
            task.Status = status;
            if (task.ParentId == null)
            {
                switch (statusName)
                {
                    case "Underway":
                        {
                            task.DateStart = DateTime.Now;
                            task.Progress = 0;
                            break;
                        }
                    case "Execute":
                        {
                            task.Progress = 99;
                            task.DueDate = DateTime.Now;
                            break;
                        }
                    case "Complete":
                        {
                            task.Progress = 100;
                            break;
                        }
                    default:
                        {
                            
                        }
                        break;
                }
            }
            else
            {
                int progress = CalculateProgressofSubTask(task.ParentId.Value);
            }
            db.Tasks.Update(task);
            db.Save();
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
