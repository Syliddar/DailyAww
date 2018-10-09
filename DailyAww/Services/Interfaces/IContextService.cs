using System.Collections.Generic;
using DailyAww.Models;

namespace DailyAww.Services.Interfaces
{
    public interface IContextService
    {
        List<Person> GetAllPeople();
        Person GetPerson(int id);
        List<Person> GetPeople(List<int> peopleIds);
        void SavePerson(Person person);
        void DeletePerson(int person);
    }
}