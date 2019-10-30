using System.Web.Mvc;
using DailyAww.Interfaces;
using DailyAww.Models;
using Hangfire;
using RedditSharp.Things;

namespace DailyAww.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IAwwService _aww;
        // GET: Schedule

        public ScheduleController(IAwwService awwService)
        {
            _aww = awwService;
        }
        public ActionResult Index()
        {
            var viewModel = new ScheduleViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SetSchedule(ScheduleViewModel viewModel)
        {
            switch (viewModel.AwwType)
            {
                case AwwTypes.HourlyAwws:
                    RecurringJob.AddOrUpdate(() => _aww.GetAwws(FromTime.Hour), viewModel.CronExpression, viewModel.TimeZoneInfo);
                    break;
                case AwwTypes.DailyAwws:
                    RecurringJob.AddOrUpdate(() => _aww.GetAwws(FromTime.Day), viewModel.CronExpression, viewModel.TimeZoneInfo);
                    break;
                case AwwTypes.WeeklyAwws:
                    RecurringJob.AddOrUpdate(() => _aww.GetAwws(FromTime.Week), viewModel.CronExpression, viewModel.TimeZoneInfo);
                    break;
            }
            return View("Index");
        }
    }
}