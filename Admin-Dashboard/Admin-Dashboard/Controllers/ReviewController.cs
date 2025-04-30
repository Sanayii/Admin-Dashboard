using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public IActionResult ManageViolations()
        {
            var reviews = Unit._ReviewRepo.UnReviewed_Reviews();
            return View(reviews);
        }   

        public IActionResult AcceptAsViolation(int id)
        {
            var review = Unit._ReviewRepo.GetById(id);
            var artisan = Unit._ArtisanRepo.GetById(review.ArtisanId);
            var contract = artisan.Contract.Id;

            var v = new Violation()
            {
                Reason = review.Comment,
                Date = DateTime.Now,
                ContractId = contract
            };

            Unit._ViolationRepo.Add(v);
            review.isReviewed = true;
            Unit._ReviewRepo.Edit(review);
            Unit.save();
            return RedirectToAction("ManageViolations");
        }
        public IActionResult IgnoreViolation(int id)
        {
            var review = Unit._ReviewRepo.GetById(id);
            review.isViolate = false;
            review.isReviewed = false;
            Unit._ReviewRepo.Edit(review);
            Unit.save();
            return RedirectToAction("ManageViolations");
        }
    }
}
