using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TaskManagerDAL.Interfaces;
using TaskManagerDAL.Entities;
using TaskManagerDAL.Context;

namespace TaskManagerDAL.Repositories
{
    public class PersonRepository : IRepository<Person>
    {
        private TaskManagerContext dataBase;

        public PersonRepository (TaskManagerContext context)
        {
            dataBase = context;
        }

        public IEnumerable<Person> GetAll()
        {
            return dataBase.People.Include(p => p.Team).Include(p => p.ApplicationUser).ToList();
        }

        public Person Get(int id)
        {
            return dataBase.People.Include(p => p.Team).Include(p => p.ApplicationUser).SingleOrDefault(p => p.Id == id);
        }

        public IEnumerable<Person> Find(Func<Person, Boolean> predicate)
        {
            return dataBase.People.Include(p => p.ApplicationUser).Include(p => p.Team).Where(predicate).ToList();
        }

        public void Create(Person person)
        {
            dataBase.People.Add(person);
        }

        public void Update(Person person)
        {
            dataBase.Entry(person).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Person person = dataBase.People.Find(id);
            if (person != null)
            {
                dataBase.People.Remove(person);
            }
        }
    }
}
