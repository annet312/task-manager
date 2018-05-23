using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Interfaces;

namespace TaskManagerBLL.Services
{
    public class TemplateSubtasksService : ITemplateSubtasksService
    {
        private readonly IUnitOfWork db;
        private readonly SubtaskService subtaskService;

        private IMapper mapper { get; set; }

        public TemplateSubtasksService(IUnitOfWork uow, SubtaskService subtaskService)
        {
            db = uow;
            this.subtaskService = subtaskService;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Person, PersonBLL>();
                cfg.CreateMap<Status, StatusBLL>();
                cfg.CreateMap<TaskTemplate, TaskTemplateBLL>();
                cfg.CreateMap<Team, TeamBLL>();
                cfg.CreateMap<Task, TaskBLL>();
            });

            mapper = config.CreateMapper();
        }
        public IEnumerable<TaskTemplateBLL> GetAllTemplates()
        {
            var result = db.TaskTemplates.Find(t => (t.TemplateId == null));
            return mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
        }

        private IEnumerable<TaskTemplateBLL> GetSubtasksOfTemplate(int templateId)
        {
            IEnumerable<TaskTemplate> result = db.TaskTemplates.Find(t => (t.TemplateId == templateId));
            return mapper.Map<IEnumerable<TaskTemplate>, IEnumerable<TaskTemplateBLL>>(result);
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
            PersonBLL author = mapper.Map<Person, PersonBLL>(db.People.Find(p => (p.Name == authorName)).SingleOrDefault());
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
                subtaskService.AddSubtask(subtask, taskId, false);
            }

            _Task parentTask = db.Tasks.Get(taskId);
            parentTask.Progress = subtaskService.CalculateProgressOfSubtask(taskId, subtaskNames.Count());
            db.Tasks.Update(parentTask);
            db.Save();
        }
    }
}
