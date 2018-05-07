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
        //IEnumerable<PersonBLL> GetManagerTeam(PersonBLL manager);
        void DeletePersonFromTeam(PersonBLL programmer);
        void AddPersonToTeam(PersonBLL programmer, TeamBLL team);
        void ChangeTeamName(int id, string NewName);
        IEnumerable<PersonBLL> GetPeopleInTeam(string managerId);
        IEnumerable<PersonBLL> GetPeopleWithoutTeam();
        PersonBLL GetPerson(int id);
        PersonBLL GetPerson(string id);
    }
}
