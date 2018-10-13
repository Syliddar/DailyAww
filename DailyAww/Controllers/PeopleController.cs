using System.Web.Mvc;
using DailyAww.Models;
using DailyAww.Services.Interfaces;

namespace DailyAww.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {
        private readonly IContextService _context;

        public PeopleController(IContextService contextService)
        {
            _context = contextService;
        }

        // GET: People
        public ActionResult Index()
        {
            var model = _context.GetAllPeople();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _context.GetPerson(id);
            return View("Update", model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new Person();
            return View("Update", model);
        }

        [HttpPost]
        public ActionResult Save(Person person)
        {
            if (ModelState.IsValid)
            {
                _context.SavePerson(person);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Remove(int id)
        {
            _context.DeletePerson(id);
            return RedirectToAction("Index");
        }
    }
}