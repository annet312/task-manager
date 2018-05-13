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

        public void DeleteTask(int taskId, string currentUserName)
        {
            TaskBLL task = null;
            try
            {
                task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            }
            catch (Exception e) //TO DO Normal exception handler
            {
                throw e;
            }
            if (task == null)
            {
                throw new ArgumentException("Task with this id not found", "taskId");
            }
            if (string.IsNullOrEmpty(currentUserName))
            {
                throw new ArgumentException("Initiator of deleting task is undefined", "currentUserName");
            }
            if (task.Author.Name == currentUserName)
            {
                if (!task.ParentId.HasValue)
                {
                    IEnumerable<TaskBLL> subtasksForDelete = GetSubtasksOfTask(task.Id);
                    foreach (var subtask in subtasksForDelete)
                    {
                        db.Tasks.Delete(subtask.Id);
                    }
                }
                else
                {
                    //then need to calculate new progress for parent task
                    _Task parrentTask = db.Tasks.Get(task.ParentId.Value);
                    parrentTask.Progress = CalculateProgressOfSubtask(task.ParentId.Value, task.Id, 100);
                    db.Tasks.Update(parrentTask);
                }
                db.Tasks.Delete(task.Id);
                db.Save();
            }
            else
            {
                throw new InvalidOperationException("Access error. You cannot delete this task");
            }
        }

        public void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true)
        {
            var task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            var status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).Single());
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
                _Task parentTask = db.Tasks.Get(taskId);
                parentTask.Progress = CalculateProgressOfSubtask(taskId, 1);
                db.Tasks.Update(parentTask);
                db.Save();//??
            }
        }
        public void AddSubtask(TaskBLL subtask, int taskId,  string authorName, bool forceToSave = true)
        { 
            //TO DO Exception
            PersonBLL author = mapper.Map<Person, PersonBLL>(db.People.Find(p => (p.Name == authorName)).Single());
            subtask.Author = author;

            AddSubtask(subtask, taskId);
        }

        public void AddSubtasksFromTemplate(int taskId, int templateId , string authorName)
        {
            IEnumerable<TaskTemplateBLL> subtaskNames = GetSubtasksOfTemplate(templateId);
            //TO DO EXCEPTIONS
            PersonBLL author = mapper.Map<Person, PersonBLL>(db.People.Find(p => (p.Name == authorName)).Single());
            
            foreach (var subtaskName in subtaskNames)
            {
                var subtask = new TaskBLL
                {
                    Name = subtaskName.Name,
                    ParentId = taskId,
                    ETA = null,
                    DueDate = null,
                    Comment = null,
                  //  Assignee = mapper.Map<_Task, TaskBLL>(parentTask).Assignee,
                    Author = author
                };
                AddSubtask(subtask, taskId, false);
            }
            _Task parentTask = db.Tasks.Get(taskId);
            parentTask.Progress = CalculateProgressOfSubtask(taskId, subtaskNames.Count());
            db.Tasks.Update(parentTask);
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
        {

            _Task taskForEdit = db.Tasks.Get(task.Id);

            if (taskForEdit != null)
            {
                if (!taskForEdit.ParentId.HasValue)
                { 
                    if ((assigneeName == null) && (task.ParentId != null))
                    {
                        //We cannot change assignee for subtask - only for main task
                        throw new ArgumentNullException("assigneeName", "Cannot be null");
                    }
                    if (taskForEdit.Assignee.Name != assigneeName)
                    {
                        Person assignee;
                        try
                        {
                            assignee = db.People.Find(p => (p.Name == assigneeName)).Single();
                        }
                        catch (InvalidOperationException e)
                        {
                            throw new InvalidOperationException("Assignee is not single in DataBase", e);
                        }
                        taskForEdit.Assignee = assignee;
                        if (taskForEdit.ParentId == null)
                        {
                            IEnumerable<_Task> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<_Task>>(GetSubtasksOfTask(taskForEdit.Id));
                            //for change assignee for all subtask of edited Task
                            foreach (var subtask in subtasks)
                            {
                                subtask.Assignee = assignee;
                                db.Tasks.Update(subtask);
                            }
                        }
                    }
                }
                taskForEdit.Name = task.Name;
                taskForEdit.ETA = task.ETA;
                taskForEdit.DueDate = task.DueDate;
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

        private int CalculateProgressOfSubtask(int mainTaskId, int numAdded)
        {
            var MainTask = db.Tasks.Get(mainTaskId);
            var subtasks = GetSubtasksOfTask(mainTaskId);
            int sumProgress = 0;
            int num = numAdded;

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

        private int CalculateProgressOfSubtask(int mainTaskId, int changedSubtaskId, int? changedSubtaskProgress)
        {
            var MainTask = db.Tasks.Get(mainTaskId);
            var subtasks = GetSubtasksOfTask(mainTaskId).Where(s => s.Id != changedSubtaskId);
            int sumProgress = changedSubtaskProgress.HasValue ? changedSubtaskProgress.Value : 0;
            int num = 1;
           
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
                        if(!task.ParentId.HasValue)
                        {
                            task.Progress = CalculateProgressOfSubtask(task.Id, 0);
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
