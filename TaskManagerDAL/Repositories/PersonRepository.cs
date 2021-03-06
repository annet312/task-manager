﻿using System;
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
            return dataBase.People;
        }

        public Person Get(int id)
        {
            return dataBase.People.Find(id);
        }

        public IEnumerable<Person> Find(Func<Person, Boolean> predicate)
        {
            return dataBase.People.Where(predicate);
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
