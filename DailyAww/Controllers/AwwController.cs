using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using DailyAww.Interfaces;
using DailyAww.Models;
using DailyAww.Services.Interfaces;
using RedditSharp.Things;

namespace DailyAww.Controllers
{
    public class AwwController : Controller
    {
        private readonly IAwwService _aww;
        private readonly ICommunicationService _comm;
        private readonly IContextService _context;
        private readonly string _adminEmail;

        public AwwController(IContextService contextService, IAwwService awwService, ICommunicationService commsService)
        {
            _adminEmail = ConfigurationManager.AppSettings["AdminEmail"];
            _context = contextService;
            _aww = awwService;
            _comm = commsService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var model = new OnDemandAwwViewModel
            {
                ModelList = _context.GetAllPeople()
                .Select(x => new OnDemandListItem
                {
                    Selected = false,
                    PersonId = x.Id,
                    PersonName = x.Name
                }).ToList();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OnDemand(OnDemandAwwViewModel model)
        {
            var emailBody = _aww.GetAwws(FromTime.Hour);
            var subject = "Emergency Awwws for You!";
            var personIdList = model.ModelList.Where(x => x.Selected)
                .Select(x => x.PersonId)
                .ToList();
            _comm.SendAwws(emailBody, subject, personIdList);
            if (ModelState.IsValidField("CustomEmail"))
                if (!string.IsNullOrEmpty(model.CustomEmail))
                {
                    var address = new MailAddress(model.CustomEmail);
                    _comm.SendAwws(emailBody, subject, address);
                }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public void DailyAwws()
        {
            List<Person> people = new List<Person>();
            string emailBody = "";
            string subject = "";
            try
            {
                people = _context.GetAllPeople();
                emailBody = _aww.GetDailyAwws();
                subject = "Awwws for " + DateTime.Today.Date.ToShortDateString();
            }
            catch (Exception ex)
            {
                _comm.SendAwws(ex.Message, "Daily Aww Failed", new MailAddress(_adminEmail));
            }
            finally
            {
                _comm.SendAwws(emailBody, subject, people);
            }
        }

        [HttpPost]
        public void WeeklyAwws()
        {
            List<Person> people = new List<Person>();
            string emailBody = "";
            string subject = "";
            try
            {
                people = _context.GetAllPeople();
                emailBody = _aww.GetWeeklyAwws();
                subject = "Saturday Edition Aww's for the week of " + DateTime.Today.AddDays(-6).ToShortDateString();
            }
            catch (Exception ex)
            {
                _comm.SendAwws(ex.Message, "Weekly Aww Failed", new MailAddress(_adminEmail));
            }
            finally
            {
                _comm.SendAwws(emailBody, subject, people);
            }
        }

        [HttpPost]
        public HttpStatusCodeResult Test()
        {
            try
            {

                var emailbody = _aww.GetHourlyAwws();
                _comm.SendAwws(emailbody, "Test Aww Message", new MailAddress(_adminEmail));
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}