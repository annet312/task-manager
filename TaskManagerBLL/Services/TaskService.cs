using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using TaskManagerBLL.Interfaces;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerBLL.Models;


namespace TaskManagerBLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork db;
        private readonly ISubtaskService subtaskService;
        private IMapper mapper { get; set; }

        public TaskService(IUnitOfWork uow, ISubtaskService subtaskService)
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

        public TaskBLL GetTask(int id)
        {
            var result = db.Tasks.Get(id);
            return mapper.Map<_Task, TaskBLL>(result);
        }

       

        #region Task 
        public void DeleteTask(int taskId, string currentUserName)
        {
            TaskBLL task = mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            if (task == null)
            {
                throw new ArgumentException("Task with this id not found", "taskId");
            }

            if (task.Author.Name == currentUserName)
            {
                if (!task.ParentId.HasValue)
                {
                    IEnumerable<TaskBLL> subtasksForDelete = subtaskService.GetSubtasksOfTask(task.Id);
                    foreach (var subtask in subtasksForDelete)
                    {
                        db.Tasks.Delete(subtask.Id);
                    }
                }
                else
                {
                    //then need to calculate new progress for parent task
                    _Task parrentTask = db.Tasks.Get(task.ParentId.Value);
                    parrentTask.Progress = subtaskService.CalculateProgressOfSubtask(task.ParentId.Value, task.Id, deleting: true);
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

        public void CreateTask(TaskBLL task, string authorName,string assigneeName )
        {
            if(string.IsNullOrWhiteSpace(authorName))
            {
                throw new ArgumentNullException("Author is not shown", "authorName");
            }
            PersonBLL authorBLL = mapper.Map<Person,PersonBLL>(db.People.Find(p => p.Name == authorName).Single());
            
            PersonBLL assigneeBLL;
            if (string.IsNullOrEmpty(assigneeName))
            {
                assigneeBLL = authorBLL;
            }
            else
            {
                assigneeBLL = mapper.Map<Person, PersonBLL>(db.People.Find(p => p.Name == assigneeName).Single());
            }

            StatusBLL status = mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).SingleOrDefault());
            if(status == null)
            {
                throw new Exception("Status \"New\" wasn't found in database");
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
                        assignee = db.People.Find(p => (p.Name == assigneeName)).Single();
                        taskForEdit.Assignee = assignee;

                        if (taskForEdit.ParentId == null)
                        {
                            subtaskService.ChangeAssigneeOfSubtasks(taskForEdit.ParentId.Value, assignee.Id);
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
        #endregion

    }
}
