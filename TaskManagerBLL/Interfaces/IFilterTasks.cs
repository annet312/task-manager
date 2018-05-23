using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    /// <summary>
    /// Service for filter tasks
    /// </summary>
    public interface IFilterTasks
    {   
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
