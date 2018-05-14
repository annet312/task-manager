using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;

namespace TaskManagerBLL.Interfaces
{
    public interface IPersonService
    {
        IEnumerable<TeamBLL> GetAllTeams();
        int CreateNewTeam(string ManagerName, string TeamName);
        void DeletePersonFromTeam(int id);
        void AddPersonsToTeam(int[] persons, string managerId);
        void ChangeTeamName(int id, string NewName);
        IEnumerable<PersonBLL> GetAssignees(string managerId);
        IEnumerable<PersonBLL> GetAssignees(int managerId);
        IEnumerable<PersonBLL> GetTeam(string managerId);
        IEnumerable<PersonBLL> GetPeopleWithoutTeam();
        PersonBLL GetPerson(int id);
        PersonBLL GetPerson(string id);
    }
}
