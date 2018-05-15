using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    /// <summary>
    /// Service for working with Persons
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Get all teams that are contained in database
        /// </summary>
        /// <returns>Enumeration of teams in type TeamBLL</returns>
        IEnumerable<TeamBLL> GetAllTeams();
        /// <summary>
        /// Delete person from team 
        /// </summary>
        /// <param name="id">Id of person which need to delete from team</param>
        /// ///<exception cref="ArgumentException">if person wasn't found</exception>
        void DeletePersonFromTeam(int id);
        /// <summary>
        /// Add persons to team of manager
        /// </summary>
        /// <param name="persons">Array of Id of persons which needed to add to team of manager</param>
        /// <param name="managerId">Id of manager who want to add members to his team</param>
        /// ///<exception cref="ArgumentException">if manager wasn't found or members wasn't found</exception>
        void AddPersonsToTeam(int[] persons, string managerId);
        /// <summary>
        /// Change name of team
        /// </summary>
        /// <param name="id"> Id of team</param>
        /// <param name="NewName">new name for team</param>
        /// ///<exception cref="ArgumentException">team wasn't found</exception>
        void ChangeTeamName(int id, string NewName);
        /// <summary>
        /// Get enumerate of assignees for manager
        /// </summary>
        /// <param name="managerId">string Id of current manager</param>
        /// <returns>Enumeration of person who can be assignee for this manager</returns>
        IEnumerable<PersonBLL> GetAssignees(string managerId);
        /// <summary>
        /// Get enumerate of assignees for manager
        /// </summary>
        /// <param name="managerId">int Id of current manager</param>
        /// <returns>Enumeration of person who can be assignee for this manager</returns>
        IEnumerable<PersonBLL> GetAssignees(int managerId);
        /// <summary>
        /// Get an enumeration of persons who are members of managers team
        /// </summary>
        /// <param name="managerId">string Id of manager</param>
        /// <returns>Enumeration of persons</returns>
        IEnumerable<PersonBLL> GetTeam(string managerId);
        /// <summary>
        /// Get an enumeration of persons who doesn't have a team
        /// </summary>
        /// <returns>Enumerations of persons</returns>
        IEnumerable<PersonBLL> GetPeopleWithoutTeam();
        /// <summary>
        /// Get person with this Id
        /// </summary>
        /// <param name="id">int Id of needed person</param>
        /// <returns>Person</returns>
        PersonBLL GetPerson(int id);
        /// <summary>
        /// Get person with this Id
        /// </summary>
        /// <param name="id">string Id of needed person</param>
        /// <returns>Person</returns>
        PersonBLL GetPerson(string id);
    }
}
