using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskMng.Models;

namespace TaskMng.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class HomeController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IPersonService personService;
        private readonly IFilterTaskService filterTaskService;
        private readonly IStatusService statusService;
        private readonly IMapper mapper;

        public HomeController(ITaskService taskService, 
                              IPersonService personService,
                              IFilterTaskService filterTaskService,
                              IStatusService statusService,
                                IMapper mapper)
        {
            this.taskService = taskService;
            this.personService = personService;
            this.filterTaskService = filterTaskService;
            this.statusService = statusService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        #region Tasks
        [HttpGet]
        public ActionResult MyTasks()
        {
            IEnumerable<TaskView> tasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(filterTaskService.GetTaskOfAssignee(User.Identity.GetUserId())).ToList();

            ViewBag.TeamTasksView = false;

            return PartialView("Tasks", tasks);
        }

        [HttpPost]
        public string DeleteTask(int id)
        {
            try
            {
                //Try to delete this task and check task id and current user Name if he can delete it
                //because deleting task is available only for author of task 
                taskService.DeleteTask(id, User.Identity.Name);
            }
            catch (Exception e)
            {
                return ("Task wasn't deleted. Error: " + e.Message);
            }
            return ("Task was deleted");
        }

        [HttpPost]
        public string CreateTask(CreateTaskView newTask)
        {
            string author = User.Identity.Name;

            if (newTask.TemplateId.HasValue)
            {
                try
                {
                    taskService.AddSubtasksFromTemplate(newTask.ParentId.Value, newTask.TemplateId.Value, author);
                }
                catch (Exception e)
                {
                    return "Error. Task wasn't created" + e.Message;
                }
                return "Task was created";
            }

            var task = new TaskView
            {
                ParentId = newTask.ParentId,
                Name = newTask.Name,
                Comment = newTask.Comment
            };

            try
            {
                if (!newTask.ParentId.HasValue)
                {
                    taskService.CreateTask(mapper.Map<TaskView, TaskBLL>(task), author, newTask.Assignee);
                }
                else
                {
                    taskService.AddSubtask(mapper.Map<TaskView, TaskBLL>(task), task.ParentId.Value);
                }
            }
            catch (Exception e)
            {
                return ("Task wasn't created. Error: " + e.Message);
            }
            return "Task was created";
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            TaskView task = mapper.Map<TaskBLL, TaskView>(taskService.GetTask(id));
            IEnumerable<TaskView> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(taskService.GetSubtasksOfTask(id));
            DetailsTaskView tasks = new DetailsTaskView
            {
                MainTask = task,
                Subtasks = subtasks
            };
            return PartialView("Details", tasks);
        }

        [HttpPost]
        public ActionResult EditTask(int id)
        {
            TaskView task = mapper.Map<TaskBLL, TaskView>(taskService.GetTask(id));
            return PartialView("EditTask", task);
        }

        [HttpPost]
        public string SaveEditTask(TaskEditView taskForEdit)
        {
            var task = new TaskBLL
            {
                Id = taskForEdit.Id,
                Name = taskForEdit.Name,
                ParentId = null,
                Comment = taskForEdit.Comment,
                ETA = taskForEdit.ETA,
                DueDate = taskForEdit.DueDate
            };
            string author = User.Identity.Name;
            string assignee = taskForEdit.Assignee ?? author;
            try
            {
                taskService.SaveChangeTask(task, assignee);
            }
            catch
            {
                return ("Error: cannot change task!");
            }
            return ("Task was changed.");
        }

        [HttpGet]
        public ActionResult ShowSubtask(int parentId)
        {
            IEnumerable<TaskView> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(taskService.GetSubtasksOfTask(parentId));
            
            ViewBag.ParentId = parentId;

            return PartialView("ShowSubtask", subtasks);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult TaskOfMyTeam()
        {
            string id = User.Identity.GetUserId();
            PersonBLL manager = personService.GetPerson(id);

            IEnumerable<TaskView> tasksOfMyTeam;
            tasksOfMyTeam = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(filterTaskService.GetTasksOfTeam(id));

            ViewBag.ManagerId = manager.Id;
            ViewBag.TeamTasksView = true;

            return PartialView("Tasks", tasksOfMyTeam);
        }
        [HttpGet]
        public ActionResult GetStatuses()
        {
            IEnumerable<StatusView> statuses;
            if (User.IsInRole("Programmer"))
            {
                statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(statusService.GetStatuses());
            }
            else
            {
                statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(statusService.GetAllStatuses());
            }
            return PartialView("StatusList", statuses);
        }

        [HttpGet]
        public ActionResult GetAssignees(int managerId)
        {
            IEnumerable<PersonView> assignees = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(personService.GetAssignees(managerId));
            return PartialView("AssigneeList", assignees);
        }

        [HttpPost]
        public HttpStatusCode SetNewStatus(int id, string status)
        {
            if (status != null)
            {
                statusService.SetNewStatus(id, status);
            }
            return HttpStatusCode.OK;
        }
        
        #endregion
        
    }
}
