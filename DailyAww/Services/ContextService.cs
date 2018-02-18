using DailyAww.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DailyAww.Models;

namespace DailyAww.Services
{
    public class ContextService : IContextService
    {
        private readonly ApplicationDbContext _db;


        public ContextService()
        {
            _db = new ApplicationDbContext();
        }
        public List<Person> GetAllPeople()
        {
            return _db.People.ToList();
        }

        public Person GetPerson(int id)
        {
            return _db.People.Find(id);
        }

        public void SavePerson(Person person)
        {
            try
            {
                if (person.Id == 0)
                {
                    _db.People.Add(person);
                }
                else
                {
                    _db.People.Attach(person);
                    _db.Entry(person).State = System.Data.Entity.EntityState.Modified;
                }
                _db.SaveChanges();
            }
            catch 
            {
                //Return Error Message at some point                
            }
        }

        public void DeletePerson(int id)
        {
            try
            {
                var person = _db.People.Find(id);
                _db.People.Remove(person);
                _db.SaveChanges();
            }
            catch
            {
                //Return Error Message at some point                
            }

        }
    }
}