using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class PersonManager : IPersonManager
    {
        private TaskManagerContext db { get; set; }
        public PersonManager(TaskManagerContext context)
        {
            db = context;
        }
        public void Create(Person person, string teamNameStr)
        {//TODO
            //var newPerson = new Person { Name = person.Name, EAdress = person.EAdress };
            db.People.Add(person);
            //var team = new Team { TeamName = teamNameStr, ManagerId = newPerson.Id };
            //db.Teams.Add(team);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
