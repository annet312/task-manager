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
            ICollection<StatusView> statuses = mapper.Map<IEnumerable<StatusBLL>, IEnumerable<StatusView>>(serviceTask.GetAllStatuses()).ToList();
            if (User.IsInRole("Programmer"))
            {
                statuses.Remove(statuses.Where(s => (s.Name == "Complete")).Single());
            }
            return PartialView("StatusList", statuses);
        }

        [HttpGet]
        public ActionResult GetAssignees(int managerId)
        {
            IEnumerable<PersonView> assignees = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetPeopleInTeam(managerId));
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
        //public ActionResult CreateTask()
        //{
        //    ViewBag.Message = "New task";
        //    return View();
        //}

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
            catch (InvalidOperationException e)
            {
                return ("Task wasn't changed because " + e.Message);
            }
            catch (Exception e)
            {
                return ("Task wasn't changed because " + e.Message);
            }
            return ("Task was changed");
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
            var tasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetTaskOfAssignee(User.Identity.GetUserId())).ToList();
            if (!tasks.Any())
            {

                ViewBag.Message = "You haven't tasks";
            }
            return PartialView("MyTasks", tasks);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult MyTeam()
        {
            var id = User.Identity.GetUserId();
            var teamOfCurrentManager = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetPeopleInTeam(id));
            var person = servicePerson.GetPerson(id);
            var team = person.Team;
            ViewBag.TeamName = (team != null) ? team.TeamName : string.Empty;
            return PartialView("MyTeam", teamOfCurrentManager.ToList());

        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult TaskOfMyTeam()
        {
            var id = User.Identity.GetUserId();
            var taskOfMyTeam = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetTasksOfTeam(id));
            return PartialView("TaskOfMyTeam", taskOfMyTeam);
        }

        [HttpPost]
        public HttpStatusCode DeleteTask(int id)
        {
            try
            {
                //Try to delete this task and send task id and current user Name for check if he can delete it
                //because deleting task is available only for author of task 
                serviceTask.DeleteTask(id, User.Identity.Name);
            }
            catch
            {
            }
            return HttpStatusCode.OK;
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
                string assignee = newTask.Assignee;
                var task = new TaskView
                {
                    ParentId = null,
                    Name = newTask.Name,
                    Comment = newTask.Comment
                };
                try
                {
                    serviceTask.CreateTask(mapper.Map<TaskView, TaskBLL>(task), author, assignee);

                }
                catch (InvalidOperationException e)
                {
                    return ("Task wasn't created because " + e.Message);
                }
                catch (Exception e)
                {
                    return ("Task wasn't created because " + e.Message);
                }

            }
            return ("Task was created");
        }

        [HttpPost]
        public HttpStatusCode AddSubtask(int parentId, int? TemplateId, CreateTaskView newSubtask)
        {
            
            string author = User.Identity.Name;

            if (TemplateId.HasValue)
            {
               
                serviceTask.AddSubtasksFromTemplate(parentId, TemplateId.Value, author);
                // TO DO exceptions
                return HttpStatusCode.OK;
            }
            
            if (newSubtask != null)
            {
            
                var task = new TaskView
                {
                    ParentId = parentId,
                    Name = newSubtask.Name,
                    Comment = newSubtask.Comment
                };
              
                serviceTask.AddSubtask(mapper.Map<TaskView,TaskBLL>(task), parentId, author);
                //TODO Exceptions
                return HttpStatusCode.OK;
            }
            return HttpStatusCode.PreconditionFailed;///???
        }
    }
}