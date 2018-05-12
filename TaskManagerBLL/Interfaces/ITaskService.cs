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
        void DeleteTask(int taskId, string currentUserName);
        void AddSubtask(TaskBLL subtask, int taskId, bool forceToSave = true);
        void AddSubtask(TaskBLL subtask, int taskId, string authorName, string assigneeName, bool forceToSave = true);
        void AddSubtasksFromTemplate(int taskId, int templateId, string authorName, string assigneeName);
        void CreateTask(TaskBLL task, string authorId, string assigneeId);
        void SaveChangeTask(TaskBLL task, string assignee);
        IEnumerable<TaskBLL> GetSubtasksOfTask(int Id);
        void SetNewStatus(int taskid, string statusName);
        TaskBLL GetTask(int id);
        IEnumerable<TaskBLL> GetTasksOfTeam(string managerId);
        IEnumerable<TaskBLL> GetOverDueTasks(int teamId);
        IEnumerable<TaskBLL> GetCompleteTasks(int teamId);
        IEnumerable<TaskBLL> GetTaskOfAssignee(string id);
    }
}
