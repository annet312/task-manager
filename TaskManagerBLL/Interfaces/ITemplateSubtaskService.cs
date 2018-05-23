using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    /// <summary>
    /// Service for working with templates of subtasks
    /// </summary>
    public interface ITemplateSubtasksService
    {
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
        /// Get all possible templates of subtasks sets
        /// </summary>
        /// <returns>Enumerations of templates</returns>
        IEnumerable<TaskTemplateBLL> GetAllTemplates();
    }
}
