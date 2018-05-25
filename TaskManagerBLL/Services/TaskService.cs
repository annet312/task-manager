using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using TaskManagerBLL.Interfaces;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerBLL.Models;


namespace TaskManagerBLL.Services
{
    public class TaskService : ITaskService
    {
        private IUnitOfWork db { get; set; }

        public TaskService(IUnitOfWork uow)
        {
            db = uow;
        }

        public TaskBLL GetTask(int id)
        {
            var result = db.Tasks.Get(id);
            return Mapper.Map<_Task, TaskBLL>(result);
        }

        #region Templates
        public IEnumerable<TaskTemplateBLL> GetAllTemplates()
        {
            var result = db.TaskTemplates.Find(t => (t.TemplateId == null));
            return Mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
        }
     
        private IEnumerable<TaskTemplateBLL> GetSubtasksOfTemplate(int templateId)
        {
            IEnumerable<TaskTemplate> result = db.TaskTemplates.Find(t => (t.TemplateId == templateId));
            return Mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
        }

        public void AddSubtasksFromTemplate(int taskId, int templateId, string authorName)
        {
            IEnumerable<TaskTemplateBLL> subtaskNames = GetSubtasksOfTemplate(templateId);
            if (!subtaskNames.Any())
            {
                throw new ArgumentException("subtasks from template with this Id wasn't found", "templateId");
            }

            if (string.IsNullOrWhiteSpace(authorName))
            {
                throw new ArgumentNullException("Name of author is null or empty", "authorName");
            }
            PersonBLL author = Mapper.Map<Person, PersonBLL>(db.People.Find(p => (p.Name == authorName)).SingleOrDefault());
            if (author == null)
            {
                throw new ArgumentException("Author with tis name wasn't found", "authorName");
            }

            foreach (var subtaskName in subtaskNames)
            {
                var subtask = new TaskBLL
                {
                    Name = subtaskName.Name,
                    ParentId = taskId,
                    ETA = null,
                    DueDate = null,
                    Comment = null,
                    Author = author
                };
                AddSubtask(subtask, taskId, false);
            }

            _Task parentTask = db.Tasks.Get(taskId);
            parentTask.Progress = CalculateProgressOfSubtask(taskId, subtaskNames.Count());
            db.Tasks.Update(parentTask);
            db.Save();
        }
        #endregion

        #region Subtask
        public IEnumerable<TaskBLL> GetSubtasksOfTask(int parentId)
        {
            var subtasks = db.Tasks.Find(t => (t.ParentId == parentId));
            return Mapper.Map<IEnumerable<_Task>, IEnumerable<TaskBLL>>(subtasks);
        }

        public void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true)
        {
            var task = Mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            if (task == null)
            {
                throw new ArgumentException("Parent task wasn't found", "taskId");
            }

            var status = Mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).Single());

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
            db.Tasks.Create(Mapper.Map<TaskBLL, _Task>(newSubtask));

            if (forceToSave)
            {
                _Task parentTask = db.Tasks.Get(taskId);
                parentTask.Progress = CalculateProgressOfSubtask(taskId, 1);
                db.Tasks.Update(parentTask);
                db.Save();
            }
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
            if (MainTask.Status.Name == "Closed")
            {
                return 100;
            }
            sumProgress = (num != 0) ? (sumProgress / num) : 0;
            return sumProgress;
        }

        private int CalculateProgressOfSubtask(int mainTaskId, int deletedSubtaskId, bool deleting)
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
        #endregion

        #region Task 
        public void DeleteTask(int taskId, string currentUserName)
        {
            TaskBLL task = Mapper.Map<_Task, TaskBLL>(db.Tasks.Get(taskId));
            if (task == null)
            {
                throw new ArgumentException("Task with this id not found", "taskId");
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
                    parrentTask.Progress = CalculateProgressOfSubtask(task.ParentId.Value, task.Id, deleting: true);
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
            PersonBLL authorBLL = Mapper.Map<Person,PersonBLL>(db.People.Find(p => p.Name == authorName).Single());
            
            PersonBLL assigneeBLL;
            if (string.IsNullOrEmpty(assigneeName))
            {
                assigneeBLL = authorBLL;
            }
            else
            {
                assigneeBLL = Mapper.Map<Person, PersonBLL>(db.People.Find(p => p.Name == assigneeName).Single());
            }

            StatusBLL status = Mapper.Map<Status, StatusBLL>(db.Statuses.Find(s => (s.Name == "New")).SingleOrDefault());
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

            db.Tasks.Create(Mapper.Map<TaskBLL, _Task>(newTask));
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
                            IEnumerable<_Task> subtasks = db.Tasks.Find(t => (t.ParentId == taskForEdit.Id));
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
        #endregion
    }
}
