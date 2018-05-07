using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<StatusBLL> GetAllStatuses();
        IEnumerable<TaskTemplateBLL> GetAllTemplates();
        IEnumerable<TaskTemplateBLL> GetSubtasksOfTemplate(int tempplateId);
        void DeleteTask(int taskId);
        void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true);
        void AddSubtasksFromTemplate(int taskId, int templateId);
        void CreateTask(TaskBLL task, string authorId, string assigneeId);
        void SaveChangeTask(TaskBLL task);
        IEnumerable<TaskBLL> GetSubtasksOfTask(int Id);
        void SetNewStatus(int statusId, TaskBLL task);
        TaskBLL GetTask(int id);
        IEnumerable<TaskBLL> GetTasksOfTeam(string managerId);
        IEnumerable<TaskBLL> GetOverDueTasks(int teamId);
        IEnumerable<TaskBLL> GetCompleteTasks(int teamId);
        IEnumerable<TaskBLL> GetTaskOfAssignee(string id);
    }
}
