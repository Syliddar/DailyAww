using DailyAww.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyAww.Interfaces
{
    public interface IContextService
    {
        List<Person> GetAllPeople();
        Person GetPerson(int id);
        void SavePerson(Person person);
        void DeletePerson(int person);
    }
}