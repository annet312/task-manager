using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskMng.Models;

namespace TaskMng.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TeamController : Controller
    {
        private readonly IPersonService personService;
        // GET: Team
        public TeamController(ITaskService taskService, IPersonService personService)
        {

            this.personService = personService;
        }
          #region Team

        [HttpGet]
        public ActionResult MyTeam()
        {
            string id = User.Identity.GetUserId();
            PersonBLL person = personService.GetPerson(id);
            TeamBLL team = person.Team;
            IEnumerable<PersonView> teamOfCurrentManager = Mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(personService.GetTeam(id));

            ViewBag.TeamName = (team != null) ? team.TeamName : string.Empty;

            return PartialView("MyTeam", teamOfCurrentManager.ToList());
        }

        [HttpGet]
        public ActionResult GetPossibleMembers()
        {
            IEnumerable<PersonView> persons = Mapper.Map<IEnumerable<PersonBLL>, IEnumerable<PersonView>>(personService.GetPeopleWithoutTeam());

            return PartialView("PossibleMembers", persons.ToList());
        }

        [HttpPost]
        public string DeleteFromTeam(int id)
        {
            try
            {
                personService.DeletePersonFromTeam(id);
            }
            catch (Exception e)
            {
                return ("Members wasn't deleted. Errors: " + e.Message);
            }
            return ("Members was deleted from your team");
        }

        
        [HttpPost]
        public string AddMembersToTeam(int[] persons)
        {
            var managerId = User.Identity.GetUserId();
            try
            {
                personService.AddPersonsToTeam(persons, managerId);
            }
            catch (Exception e)
            {
                return ("Members wasn't added. Error: " + e.Message);
            }
            return ("Members was added to your team");
        }

        #endregion
    }
}
