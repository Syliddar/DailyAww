using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DailyAww.Models;
using DailyAww.Services.Interfaces;

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

        public List<Person> GetPeople(List<int> peopleIds)
        {
            return _db.People.Where(x => peopleIds.Contains(x.Id)).ToList();
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
                    _db.Entry(person).State = EntityState.Modified;
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