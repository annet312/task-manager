using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagerBLL.Interfaces;
using AutoMapper;
using TaskManagerBLL.Infrastructure;
using Ninject;
using TaskManagerBLL.Models;
using TaskMng.Models;
using Microsoft.AspNet.Identity;

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
            //if (User.IsInRole("Manager"))
            //{
            //    var id = User.Identity.GetUserId();
            //    var teamOfCurrentManager = mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(servicePerson.GetPeopleInTeam(id));
            //    var team = servicePerson.GetPerson(id).Team;
            //    ViewBag.TeamName = team == null ? "" : team.TeamName;
            //    return View("MyTeam", teamOfCurrentManager.ToList());
            //}
            //else
            //{
            //    var tasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetTaskOfAssignee(User.Identity.GetUserId()));
            //    return View("MyTask", tasks.ToList());
            //}
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
            List<TaskView> tasks = subtasks.Prepend(task).ToList();
            //View will take collection of tasks where first element - main task ;
            //the rest - subtasks if they are exist
            return View("Details", tasks);
        }
        [HttpGet]
        public ActionResult ShowSubtask(int parentId)
        {
            IEnumerable<TaskView> subtasks = mapper.Map<IEnumerable<TaskBLL>, IEnumerable<TaskView>>(serviceTask.GetSubtasksOfTask(parentId));
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
        public void DeleteTask(int id)
        {
            serviceTask.DeleteTask(id);
        }
        [HttpPost]
        public bool CreateTask(TaskView newTask)
        {
            if (ModelState.IsValid)
            {
                PersonView author = mapper.Map<PersonBLL, PersonView>(servicePerson.GetPerson(User.Identity.GetUserId()));
                var task = new TaskView
                {
                    ParentId = null,
                    Author = author,
                    Assignee = newTask.Assignee,
                    Name = newTask.Name,
                    ETA = newTask.ETA,
                    DueDate = newTask.DueDate,
                    Comment = newTask.Comment
                };
                try
                {
                    serviceTask.CreateTask(mapper.Map<TaskView, TaskBLL>(task));
                    return true;
                }
                catch
                {
                    return false;
                }
               // return RedirectToAction("Index", "Home"); 

            }
            return false;
        }
       
    }
}