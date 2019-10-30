using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using DailyAww.Models;
using DailyAww.Services.Interfaces;

namespace DailyAww.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IContextService _context;
        private readonly string _serviceEmailAddress;
        private readonly string _serviceHost;
        private readonly string _servicePassword;
        private readonly int _servicePort;

        public CommunicationService(IContextService context)
        {
            _context = context;
            _serviceEmailAddress = ConfigurationManager.AppSettings["ServiceEmailAddress"];
            _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];
            _serviceHost = ConfigurationManager.AppSettings["ServiceHost"];
            _servicePort = Convert.ToInt32(ConfigurationManager.AppSettings["ServicePort"]);
        }

        public void SendAwws(string message, string subject, IEnumerable<Person> peopleList)
        {
            var awwMail = new MailMessage
            {
                From = new MailAddress(_serviceEmailAddress),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };
            foreach (var person in peopleList) awwMail.Bcc.Add(new MailAddress(person.EmailAddress, person.Name));
            SmtpClientSend(awwMail);
        }

        public void SendAwws(string message, string subject, MailAddress customAddress)
        {
            var awwMail = new MailMessage
            {
                From = new MailAddress(_serviceEmailAddress),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };
            awwMail.To.Add(customAddress);
            SmtpClientSend(awwMail);
        }

        public void SendAwws(string message, string subject, List<int> personIdList)
        {
            var personList = _context.GetPeople(personIdList);
            var awwMail = new MailMessage
            {
                From = new MailAddress(_serviceEmailAddress),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };
            foreach (var person in personList) awwMail.Bcc.Add(new MailAddress(person.EmailAddress, person.Name));
            SmtpClientSend(awwMail);
        }

        private void SmtpClientSend(MailMessage awwMail)
        {
            using (var client = new SmtpClient(_serviceHost, _servicePort))
            {
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_serviceEmailAddress, _servicePassword, "miamemphis.org");
                client.Timeout = 20000;
                client.Send(awwMail);
            }
        }
    }
}