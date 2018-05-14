using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TaskManagerBLL.Infrastructure;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskMng.Models;

namespace TaskMng.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ITaskService serviceTask { get; set; }
        private IPersonService servicePerson { get; set; }
        private IMapper mapper { get; set; }

        public HomeController()
        {
            var f = new ServiceModule();
            IKernel ninjectKernel = new StandardKernel(f);
            serviceTask = ninjectKernel.Get<ITaskService>();
            servicePerson = ninjectKernel.Get<IPersonService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamBLL, TeamView>();
                cfg.CreateMap<StatusBLL, StatusView>();
                cfg.CreateMap<TaskBLL, TaskView>();
                cfg.CreateMap<PersonBLL, PersonView>();
            });

            mapper = config.CreateMapper();
        }

        public ActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public ActionResult GetStatuses()
        {
            IEnumerable<StatusView> statuses;
            if (User.IsInRole("Programmer"))
            {
                statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(serviceTask.GetStatuses());
            }
            else
            {
                statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(serviceTask.GetAllStatuses());
            }
            return PartialView("StatusList", statuses);
        }

        [HttpGet]
        public ActionResult GetAssignees(int managerId)
        {
            IEnumerable<PersonView> assignees = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetAssignees(managerId));
            return PartialView("AssigneeList", assignees);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Statuses";
            var statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(serviceTask.GetAllStatuses());
            return View(statuses.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            TaskView task = mapper.Map<TaskBLL, TaskView>(serviceTask.GetTask(id));
            IEnumerable<TaskView> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetSubtasksOfTask(id));
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
            TaskView task = mapper.Map<TaskBLL, TaskView>(serviceTask.GetTask(id));
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
                serviceTask.SaveChangeTask(task, assignee);
            }
            catch (Exception e)
            {
                return ("Error: cannot change task!");
            }
            return ("Task was changed.");
        }

        [HttpGet]
        public ActionResult ShowSubtask(int parentId)
        {
            IEnumerable<TaskView> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetSubtasksOfTask(parentId));

            ViewBag.ParentId = parentId;

            return PartialView("ShowSubtask", subtasks);
        }

        [HttpGet]
        public ActionResult MyTasks()
        {
            IEnumerable<TaskView> tasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetTaskOfAssignee(User.Identity.GetUserId())).ToList();

            ViewBag.TeamTasksView = false;

            return PartialView("Tasks", tasks);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult MyTeam()
        {
            string id = User.Identity.GetUserId();
            PersonBLL person = servicePerson.GetPerson(id);
            TeamBLL team = person.Team;
            IEnumerable<PersonView> teamOfCurrentManager = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetTeam(id));

            ViewBag.TeamName = (team != null) ? team.TeamName : string.Empty;

            return PartialView("MyTeam", teamOfCurrentManager.ToList());
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult GetPossibleMembers()
        {
            IEnumerable<PersonView> persons = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetPeopleWithoutTeam());

            return PartialView("PossibleMembers", persons.ToList());
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult TaskOfMyTeam()
        {
            string id = User.Identity.GetUserId();
            PersonBLL manager = servicePerson.GetPerson(id);

            IEnumerable<TaskView> tasksOfMyTeam = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetTasksOfTeam(id));

            ViewBag.ManagerId = manager.Id;
            ViewBag.TeamTasksView = true;

            return PartialView("Tasks", tasksOfMyTeam);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public string DeleteFromTeam (int id)
        {
            try
            {
                servicePerson.DeletePersonFromTeam(id);
            }
            catch (Exception e)
            {
                return ("Members wasn't deleted. Errors: " + e.Message);
            }
            return ("Members was deleted from your team");
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public string AddMembersToTeam(int[] persons)
        {
            var managerId = User.Identity.GetUserId();
            try
            {
                servicePerson.AddPersonsToTeam(persons, managerId);
            }
            catch(Exception e)
            {
                return ("Members wasn't added. Error: " + e.Message);
            }
            return ("Members was added to your team");
        }

        [HttpPost]
        public string DeleteTask(int id)
        {
            try
            {
                //Try to delete this task and send task id and current user Name for check if he can delete it
                //because deleting task is available only for author of task 
                serviceTask.DeleteTask(id, User.Identity.Name);
            }
            catch (Exception e)
            {
                return ("Task wasn't deleted. Error: " + e.Message);
            }
            return ("Task was deleted");
        }
        [HttpPost]
        public HttpStatusCode SetNewStatus(int id, string status)
        {
            if (status != null)
            {
                serviceTask.SetNewStatus(id, status);
            }
            return HttpStatusCode.OK;
        }
        [HttpPost]
        public string CreateTask(CreateTaskView newTask)
        {
            if (ModelState.IsValid)
            {
                //PersonView author = mapper.Map<PersonBLL, PersonView>(servicePerson.GetPerson(User.Identity.GetUserId()));
                string author = User.Identity.Name;

                if (newTask.TemplateId.HasValue)
                {
                    serviceTask.AddSubtasksFromTemplate(newTask.ParentId.Value, newTask.TemplateId.Value, author);
                    // TO DO exceptions
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
                        serviceTask.CreateTask(mapper.Map<TaskView, TaskBLL>(task), author, newTask.Assignee);
                    }
                    else
                    {
                        serviceTask.AddSubtask(mapper.Map<TaskView, TaskBLL>(task), task.ParentId.Value);
                    }
                }
                catch (InvalidOperationException e)
                {
                    return ("Task wasn't created. Error: " + e.Message);
                }
                catch (Exception e)
                {
                    return ("Task wasn't created. Error: " + e.Message);
                }
            }

            return "Task was created";
        }
    }
}
