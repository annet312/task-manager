using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
/// <summary>
/// Service for working with tasks 
/// </summary>
    public interface ITaskService
    {
      
        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="taskId">Id of task for deleting</param>
        /// <param name="currentUserName">Name of person who try to delete this task</param>
        /// ///<exception cref="ArgumentException">Task with this Id wasn't found</exception>
        /// ///<exception cref="InvalidOperationException">If person who want to delete task isn't its author</exception>
        void DeleteTask(int taskId, string currentUserName);
       
        /// <summary>
        /// Add task to database
        /// </summary>
        /// <param name="task">Task to adding</param>
        /// <param name="authorName">Name of tasks author</param>
        /// <param name="assigneeName">Name of tasks assignee if null -  assignee will be an author</param>
        /// ///<exception cref="ArgumentNullException">Name of author is null or empty</exception>
        /// ///<exception cref="Exception">Problem with status new in database. </exception>  
        void CreateTask(TaskBLL task, string authorName, string assigneeName);
        /// <summary>
        /// Save task which was edit by user
        /// </summary>
        /// <param name="task">Task with edits</param>
        /// <param name="assigneeName">Name of assignee of task</param>
        /// ///<exception cref="ArgumentNullException">Name of assignee is null or empty</exception>  
        void SaveChangeTask(TaskBLL task, string assigneeName);

        /// <summary>
        /// get task with Id
        /// </summary>
        /// <param name="id">Id of task</param>
        /// <returns>Task</returns>
        TaskBLL GetTask(int id);

    }
}
