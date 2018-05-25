
using System.Collections.Generic;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    public interface IStatusService
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
        /// set new status to task
        /// </summary>
        /// <param name="taskid">Id of task that need to set new status</param>
        /// <param name="statusName">Name of new status for task</param>
        /// ///<exception cref="ArgumentNullException">Name of status is null or empty</exception>
        /// ///<exception cref="ArgumentException">task for edit wasn't found</exception>   
        void SetNewStatus(int taskid, string statusName);
    }
}
