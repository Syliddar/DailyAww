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
        private readonly string _serviceAddress;
        private readonly string _serviceClient;
        private readonly string _servicePassword;
        private readonly int _servicePort;

        public CommunicationService(IContextService context)
        {
            _context = context;
            _serviceAddress = ConfigurationManager.AppSettings["ServiceAddress"];
            _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];
            _serviceClient = ConfigurationManager.AppSettings["ServiceClient"];
            _servicePort = Convert.ToInt32(ConfigurationManager.AppSettings["ServicePort"]);
        }

        public void SendAwws(string message, string subject, IEnumerable<Person> peopleList)
        {
            var awwMail = new MailMessage
            {
                From = new MailAddress(_serviceAddress),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };
            foreach (var person in peopleList) awwMail.To.Add(new MailAddress(person.EmailAddress, person.Name));
            SmtpClientSend(awwMail);
        }

        public void SendAwws(string message, string subject, MailAddress customAddress)
        {
            var awwMail = new MailMessage
            {
                From = new MailAddress(_serviceAddress),
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
                From = new MailAddress(_serviceAddress),
                Body = message,
                Subject = subject,
                IsBodyHtml = true
            };
            foreach (var person in personList) awwMail.To.Add(new MailAddress(person.EmailAddress, person.Name));
            SmtpClientSend(awwMail);
        }

        private void SmtpClientSend(MailMessage awwMail)
        {
            var client = new SmtpClient(_serviceClient, _servicePort)
            {
                Credentials = new NetworkCredential(_serviceAddress, _servicePassword),
                EnableSsl = true
            };
            client.Send(awwMail);
        }
    }
}