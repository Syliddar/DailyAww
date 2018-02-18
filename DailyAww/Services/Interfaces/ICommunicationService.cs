using DailyAww.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace DailyAww.Interfaces
{
    public interface ICommunicationService
    {
        void SendAwws(string message, string Subject, List<Person> peopleList);
        void SendAwws(string message, string Subject, MailAddress address);
        void SendAwws(string message, string Subject, List<int> personIdList);}
}