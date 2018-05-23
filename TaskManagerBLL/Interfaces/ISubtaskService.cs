using System.Collections.Generic;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    /// <summary>
    /// Service for working with subtasks
    /// </summary>
    public interface ISubtaskService
    {
        /// <summary>
        /// Get enumeration of subtasks of task
        /// </summary>
        /// <param name="parentId">Id of parent task</param>
        /// <returns>Enumeration of subtasks</returns>
        IEnumerable<TaskBLL> GetSubtasksOfTask(int parentId);
        /// <summary>
        /// Add subtask to task 
        /// </summary>
        /// <param name="subtask">Subtask which needed to add</param>
        /// <param name="taskId">Id of parent task</param>
        /// <param name="forceToSave">false if not need to save (if need to add more then 1 subtask)</param>
        /// ///<exception cref="ArgumentException">Parent task wasn't found</exception>      
        void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true);
        /// <summary>
        /// Calculate progress for parent task after adding subtasks
        /// </summary>
        /// <param name="mainTaskId">id of parent task</param>
        /// <param name="numAddedSubtasks">number added subtasks</param>
        /// <returns>progress</returns>
        int CalculateProgressOfSubtask(int mainTaskId, int numAddedSubtasks);
        /// <summary>
        /// Calculate progress for parent task after changed progress of subtask
        /// </summary>
        /// <param name="mainTaskId">id of parent task</param>
        /// <param name="changedSubtaskId">id of changed subtask</param>
        /// <param name="changedSubtaskProgress">new progress of subtask</param>
        /// <returns>progress</returns>
        int CalculateProgressOfSubtask(int mainTaskId, int changedSubtaskId, int? changedSubtaskProgress);
        /// <summary>
        /// Calculate progress for parent task after deleting subtasks
        /// </summary>
        /// <param name="mainTaskId">id of parent task</param>
        /// <param name="numAddedSubtasks">id of deleted subtask</param>
        /// <param name="deleting">flag true if subtask was deleted</param>
        /// <returns>progress</returns>
        int CalculateProgressOfSubtask(int mainTaskId, int deletedSubtaskId, bool deleting);
        /// <summary>
        /// Changing assignee for subtask
        /// </summary>
        /// <param name="parentId">id of parent Task</param>
        /// <param name="assigneeid">Id of new assignee</param>
        void ChangeAssigneeOfSubtasks(int parentId, int assigneeId);
    }
}
