using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using TaskManagerBLL.Interfaces;
using TaskManagerBLL.Models;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;

namespace TaskManagerBLL.Services
{
    /// <summary>
    /// Service for operation with task 
    /// </summary>
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork db;

        private readonly IEmailService emailService;

        
        /// <summary>
        /// DI to  database repository
        /// </summary>
        /// <param name="uow">point to context of DataBase</param>
        public PersonService(IUnitOfWork uow, IEmailService emailService)
        {
            db = uow;
            this.emailService = emailService;

        }

        #region Team
        public void ChangeTeamName(int teamId ,string NewName)
        {
            var team = db.Teams.Get(teamId);
            if(team == null)
            {
                throw new ArgumentException("Invalid Team for changes");
            }
            if(team.TeamName != NewName)
            {
                db.Teams.Delete(teamId);
                team.TeamName = NewName;
                db.Teams.Create(team);
            }
        }

        public IEnumerable<TeamBLL> GetAllTeams()
        {
            var result = db.Teams.GetAll();
            return Mapper.Map<IEnumerable<Team>, IEnumerable<TeamBLL>>(result);
        }

        public void DeletePersonFromTeam(int id)
        {
            Person person = db.People.Get(id);
            if (person == null)
            {
                throw new ArgumentException("Person is not exist");
            }
            person.TeamId = null;
            db.People.Update(person);
            db.Save();
        }

        public void AddPersonsToTeam(int[] persons, string managerId)
        {
            if ((persons == null) || (persons.Length == 0))
            {
                throw new ArgumentException("No persons to adding", "persons");
            }

            if (string.IsNullOrWhiteSpace(managerId))
            {
                throw new ArgumentException("Unknown manager", "managerId");
            }

            Person manager = db.People.Find(m => (m.UserId == managerId)).SingleOrDefault();
            if (manager == null)
            {
                throw new ArgumentException("Unknown manager", "managerId");
            }

            IEnumerable<Person> newTeamMembers = db.People.Find(p => persons.Contains(p.Id));
            if (newTeamMembers.Count() != persons.Length)
            {
                throw new ArgumentException("Not all members was founded");
            }

            foreach (var member in newTeamMembers)
            {
                member.TeamId = manager.TeamId;
                db.People.Update(member);
            }
            db.Save();

            foreach (var member in newTeamMembers)
            {
                string emailBody = string.Format(EmailService.BODY_NEW_TEAM_MEMBER,
                                                 member.Name, manager.Team.TeamName, manager.Name);
                emailService.Send(manager.Email, member.Email, EmailService.SUBJECT_NEW_TEAM_MEMBER, emailBody);
            }
        }

        public IEnumerable<PersonBLL> GetPeopleWithoutTeam()
        {
            var people = db.People.Find(p => (p.TeamId == null));
            return Mapper.Map<IEnumerable<Person>, IEnumerable<PersonBLL>>(people);
        }

        public IEnumerable<PersonBLL> GetTeam(string managerId)
        {
            var manager = db.People.Find(p =>( p.UserId == managerId)).SingleOrDefault();
            var people = GetPeopleInTeam(manager).Where(p => (p.Id != manager.Id));
            return people;
        }

        private IEnumerable<PersonBLL> GetPeopleInTeam(Person manager)
        {
            IEnumerable<PersonBLL> people = new List<PersonBLL>();
            if (manager == null)
            {
                throw new ArgumentException("Manager is not found", "managerId");
            }
            if (manager.Team == null)
            {
                return people;
            }
            people = Mapper.Map<IEnumerable<Person>, IEnumerable<PersonBLL>>(db.People.Find(p => ((p.Team != null) && (p.Team.Id == manager.Team.Id))));
            return people;
        }
        #endregion

        #region GetPerson
        public PersonBLL GetPerson(int id)
        {
            var person = db.People.Get(id);
            return Mapper.Map<Person, PersonBLL>(person);
        }

        public PersonBLL GetPerson(string id)
        {
            var person = db.People.Find(p => p.UserId == id).SingleOrDefault();
            return Mapper.Map<Person, PersonBLL>(person);
        }
        #endregion

        #region GetAssignee
        public IEnumerable<PersonBLL> GetAssignees(string managerId)
        {
            var manager = db.People.Find(p => p.UserId == managerId).SingleOrDefault();
            var people = GetPeopleInTeam(manager);
            return people;
        }

        public IEnumerable<PersonBLL> GetAssignees(int managerId)
        {
            var manager = db.People.Find(p => p.Id == managerId).SingleOrDefault();
            var people = GetPeopleInTeam(manager);
            return people;
        }
        #endregion
    }
}
