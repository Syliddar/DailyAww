using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using DailyAww.Interfaces;
using DailyAww.Models;
using DailyAww.Services.Interfaces;

namespace DailyAww.Controllers
{
    public class AwwController : Controller
    {
        private readonly IAwwService _aww;
        private readonly ICommunicationService _comm;
        private readonly IContextService _context;

        public AwwController(IContextService contextService, IAwwService awwService, ICommunicationService commsService)
        {
            _context = contextService;
            _aww = awwService;
            _comm = commsService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var model = new OnDemandAwwViewModel();
            model.ModelList = _context.GetAllPeople()
                .Select(x => new OnDemandListItem
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
            var emailBody = _aww.GetHourlyAwws();
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
            var people = _context.GetAllPeople();
            var emailBody = _aww.GetDailyAwws();
            var subject = "Awwws for " + DateTime.Today.Date.ToShortDateString();
            _comm.SendAwws(emailBody, subject, people);
        }

        [HttpPost]
        public void WeeklyAwws()
        {
            var people = _context.GetAllPeople();
            var emailBody = _aww.GetWeeklyAwws();
            var subject = "Saturday Edition Aww's for the week of " + DateTime.Today.AddDays(-6).ToShortDateString();
            _comm.SendAwws(emailBody, subject, people);
        }

        [HttpPost]
        public HttpStatusCodeResult Test()
        {
            try
            {
                var emailbody = _aww.GetDailyAwws();
                _comm.SendAwws(emailbody, "Test Aww Message", new MailAddress("Jason.Myers8@gmail.com"));
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}