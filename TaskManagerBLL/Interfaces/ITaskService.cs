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
        /// get statuses for tasks for all users who are not a manager
        /// </summary>
        /// <returns>Enumeration of statuses</returns>
        IEnumerable<StatusBLL> GetStatuses();
        /// <summary>
        /// Get all possible statuses for task
        /// </summary>
        /// <returns>Enumeration of statuses</returns>
        IEnumerable<StatusBLL> GetAllStatuses();
        /// <summary>
        /// Get all possible templates of subtasks sets
        /// </summary>
        /// <returns>Enumerations of templates</returns>
        IEnumerable<TaskTemplateBLL> GetAllTemplates();
        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="taskId">Id of task for deleting</param>
        /// <param name="currentUserName">Name of person who try to delete this task</param>
        /// ///<exception cref="ArgumentException">Task with this Id wasn't found</exception>
        /// ///<exception cref="InvalidOperationException">If person who want to delete task isn't its author</exception>
        void DeleteTask(int taskId, string currentUserName);
        /// <summary>
        /// Add subtask to task 
        /// </summary>
        /// <param name="subtask">Subtask which needed to add</param>
        /// <param name="taskId">Id of parent task</param>
        /// <param name="forceToSave">false if not need to save (if need to add more then 1 subtask)</param>
        /// ///<exception cref="ArgumentException">Parent task wasn't found</exception>      
        void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true);
        /// <summary>
        /// Add set of subtasks to task
        /// </summary>
        /// <param name="taskId">Id of parent task</param>
        /// <param name="templateId">Id of template</param>
        /// <param name="authorNames">Name of author of subtask</param>
        /// ///<exception cref="ArgumentNullException">Template or author weren't found</exception> 
        /// ///<exception cref="ArgumentException">Template or author weren't found</exception>  
        void AddSubtasksFromTemplate(int taskId, int templateId, string authorNames);
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
        /// Get enumeration of subtasks of task
        /// </summary>
        /// <param name="parentId">Id of parent task</param>
        /// <returns>Enumeration of subtasks</returns>
        IEnumerable<TaskBLL> GetSubtasksOfTask(int parentId);
        /// <summary>
        /// set new status to task
        /// </summary>
        /// <param name="taskid">Id of task that need to set new status</param>
        /// <param name="statusName">Name of new status for task</param>
        /// ///<exception cref="ArgumentNullException">Name of status is null or empty</exception>
        /// ///<exception cref="ArgumentException">task for edit wasn't found</exception>   
        void SetNewStatus(int taskid, string statusName);
        /// <summary>
        /// get task with Id
        /// </summary>
        /// <param name="id">Id of task</param>
        /// <returns>Task</returns>
        TaskBLL GetTask(int id);
        /// <summary>
        /// Get tasks of team of Manager with shown Id
        /// </summary>
        /// <param name="managerId">Id of manager of team</param>
        /// ///<exception cref="ArgumentException">Manager with this Id wasn't found</exception>   
        /// <returns>Enumeration of tasks</returns>
        IEnumerable<TaskBLL> GetTasksOfTeam(string managerId);
        /// <summary>
        /// Get tasks of team where Due date is in past
        /// </summary>
        /// <param name="teamId">Id of team</param>
        /// <returns>Enumeration of tasks</returns>
        IEnumerable<TaskBLL> GetOverDueTasks(int teamId);
        /// <summary>
        /// Get tasks of team where status is Closed
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        IEnumerable<TaskBLL> GetCompleteTasks(int teamId);
        /// <summary>
        /// Get all tasks without subtasks of shown assignee  
        /// </summary>
        /// <param name="id">string Id of assignee</param>
        /// <returns>Enumerations of tasks</returns>
        IEnumerable<TaskBLL> GetTaskOfAssignee(string id);
    }
}
