using DailyAww.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DailyAww.Models;
using System.Net.Mail;
using RedditSharp.Things;
using System.Net;
using System.Configuration;
using System.Diagnostics;

namespace DailyAww.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly string ServiceAddress;
        private readonly string ServicePassword;
        private readonly string ServiceClient;
        private readonly int ServicePort;

        public CommunicationService()
        {
            ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"];
            ServicePassword = ConfigurationManager.AppSettings["ServicePassword"];
            ServiceClient = ConfigurationManager.AppSettings["ServiceClient"];
            ServicePort = Convert.ToInt32(ConfigurationManager.AppSettings["ServicePort"]);
        }

        public void SendAwws(string message, string Subject, List<Person> peopleList)
        {
            var AwwMail = new MailMessage
            {
                From = new MailAddress(ServiceAddress),
                Body = message,
                Subject = Subject,
                IsBodyHtml = true
            };
            foreach (var person in peopleList)
            {
                AwwMail.To.Add(new MailAddress(person.EmailAddress, person.Name));
            }
            SmtpClientSend(AwwMail);
        }

        public void SendAwws(string message, string Subject, MailAddress CustomAddress)
        {
            var AwwMail = new MailMessage
            {
                From = new MailAddress(ServiceAddress),
                Body = message,
                Subject = Subject,
                IsBodyHtml = true
            };
            AwwMail.To.Add(CustomAddress);
#if DEBUG
            EventLog eventLog = new EventLog {Source = "DailyAwwLog"};
            eventLog.WriteEntry(AwwMail.To.ToString(), EventLogEntryType.Information);
            eventLog.WriteEntry(AwwMail.Body, EventLogEntryType.Information);
#else
            SmtpClientSend(AwwMail);
#endif

        }

        public void SendAwws(string message, string Subject, List<int> personIdList)
        {
            var AwwMail = new MailMessage
            {
                From = new MailAddress(ServiceAddress),
                Body = message,
                Subject = Subject,
                IsBodyHtml = true
            };
            var _db = new ApplicationDbContext();
            foreach (var personId in personIdList)
            {
                var person = _db.People.Find(personId);
                AwwMail.To.Add(new MailAddress(person.EmailAddress, person.Name));
            }
            SmtpClientSend(AwwMail);
        }

        private void SmtpClientSend(MailMessage AwwMail)
        {
            var client = new SmtpClient(ServiceClient, ServicePort)
            {
                Credentials = new NetworkCredential(ServiceAddress, ServicePassword),
                EnableSsl = true
            };
            //client.Send(AwwMail);
        }
    }
}