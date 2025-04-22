using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class ReviewController : Controller
    {
        private readonly UnitOFWork Unit;
        public ReviewController(UnitOFWork unit)
        {
            Unit = unit;
        }
        public IActionResult Index()
        {
            var reviews = Unit._ReviewRepo.GetAll();
            return View(reviews);
        }
        public IActionResult Create()
        {
            return View();
        }


        public IActionResult Delete(int id)
        {
            Unit._ReviewRepo.Delete(id);
            Unit.save();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var review = Unit._ReviewRepo.GetById(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }
    }
}
