﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class TaskRepository : IRepository<_Task>
    {
        private TaskManagerContext dataBase;

        public TaskRepository(TaskManagerContext context)
        {
            dataBase = context;
        }

        public IEnumerable<_Task> GetAll()
        {
            return dataBase.Tasks;
        }

        public _Task Get(int id)
        {
            return dataBase.Tasks.Find(id);
        }

        public IEnumerable<_Task> Find(Func<_Task, Boolean> predicate)
        {
            return dataBase.Tasks.Where(predicate);
        }

        public void Create(_Task task)
        {
            var taskForCreate = new _Task
            {
                Name = task.Name,
                ParentId = task.ParentId,
                AuthorId = task.Author.Id,
                AssigneeId = task.Assignee.Id,
                StatusId = task.Status.Id,
                Progress = task.Progress,
                DateStart = task.DateStart,
                ETA = task.ETA,
                DueDate = task.DueDate,
                Comment = task.Comment
            };

            dataBase.Tasks.Add(taskForCreate);
        }

        public void Update(_Task task)
        {
            dataBase.Entry(task).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            _Task task = dataBase.Tasks.Find(id);
            if (task != null)
            {
                dataBase.Tasks.Remove(task);
            }
        }
    }
}
