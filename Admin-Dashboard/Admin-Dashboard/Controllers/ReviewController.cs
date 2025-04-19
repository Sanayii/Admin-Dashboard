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
            var reviews = Unit._reviewRepo.getAll();
            return View(reviews);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                Unit._reviewRepo.add(review);
                Unit.save();
                return RedirectToAction("Index");
            }
            return View(review);
        }
        public IActionResult Edit(int id)
        {
            var review = Unit._reviewRepo.getById(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }
        [HttpPost]
        public IActionResult Edit(int id, Review review)
        {
            if (id != review.Id) return NotFound();
            if (ModelState.IsValid)
            {
                Unit._reviewRepo.edit(review);
                Unit.save();
                return RedirectToAction("Index");
            }
            return View(review);
        }
        public IActionResult Delete(int id)
        {
            Unit._reviewRepo.delete(id);
            Unit.save();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var review = Unit._reviewRepo.getById(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }
    }
}
