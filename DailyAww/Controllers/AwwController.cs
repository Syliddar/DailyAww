using DailyAww.Interfaces;
using DailyAww.Models;
using DailyAww.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DailyAww.Controllers
{
    public class AwwController : Controller
    {
        private readonly IContextService _context;
        private readonly IAwwService _aww;
        private readonly ICommunicationService _comm;

        public AwwController()
        {
            _context = new ContextService();
            _aww = new AwwService();
            _comm = new CommunicationService();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var model = new OnDemandAwwViewModel();
            model.ModelList = _context.GetAllPeople()
                .Select(x => new OnDemandListItem()
                {
                    Selected = false,
                    PersonId = x.Id,
                    PersonName = x.Name
                })
                .ToList();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OnDemand(OnDemandAwwViewModel model)
        {

            string emailBody = _aww.GetHourlyAwws();
            string subject = "Emergency Awwws for You!";
            List<int> personIdList = model.ModelList.Where(x => x.Selected == true)
                .Select(x => x.PersonId)
                .ToList();
            _comm.SendAwws(emailBody, subject, personIdList);
            if (ModelState.IsValidField("CustomEmail"))
            {
                if (!string.IsNullOrEmpty(model.CustomEmail))
                {
                    var address = new MailAddress(model.CustomEmail);
                    _comm.SendAwws(emailBody, subject, address);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public void DailyAwws()
        {
            List<Person> people = _context.GetAllPeople();
            string emailBody = _aww.GetDailyAwws();
            string subject = "Awwws for " + DateTime.Today.Date.ToShortDateString();
            _comm.SendAwws(emailBody, subject, people);
        }
        
        [HttpPost]
        public void WeeklyAwws()
        {
            List<Person> people = _context.GetAllPeople();
            string emailBody = _aww.GetWeeklyAwws();
            string subject = "Saturday Edition Aww's for the week of " + DateTime.Today.AddDays(-6).ToShortDateString();
            _comm.SendAwws(emailBody, subject, people);
        }
    }
}