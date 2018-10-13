using System.Collections.Generic;
using System.Net.Mail;
using DailyAww.Models;

namespace DailyAww.Services.Interfaces
{
    public interface ICommunicationService
    {
        void SendAwws(string message, string subject, MailAddress customAddress);
        void SendAwws(string message, string subject, List<int> personIdList);
        void SendAwws(string message, string subject, IEnumerable<Person> peopleList);
    }
}