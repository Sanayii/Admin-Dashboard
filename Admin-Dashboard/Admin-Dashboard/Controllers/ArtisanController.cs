using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Repository;

namespace Admin_Dashboard.Controllers
{
    public class ArtisanController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        private readonly ILogger<ArtisanController> _logger;

        public ArtisanController(UnitOFWork unitOfWork, ILogger<ArtisanController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var artisans = _unitOfWork._artisanRepo.getAll();
            return View(artisans);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var artisan = _unitOfWork._artisanRepo.getById(id);
            var user = _unitOfWork._customerRepo.db.Users
                .Include(u => u.UserPhones)
                .FirstOrDefault(u => u.Id == id);

            if (artisan == null || user == null)
                return NotFound();

            var viewModel = new ArtisanUserViewModel
            {
                Artisan = artisan,
                User = user
            };

            ViewBag.Categories = _unitOfWork._categoryRopo.getAll();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ArtisanUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Artisan.Id = viewModel.User.Id;

                _unitOfWork._artisanRepo.edit(viewModel.Artisan);
                _unitOfWork._customerRepo.db.Users.Update(viewModel.User);

                foreach (var phone in viewModel.User.UserPhones)
                {
                    _unitOfWork._customerRepo.db.UserPhones.Update(phone);
                }

                _unitOfWork.save();
                return RedirectToAction("Index");
            }

            _logger.LogError("ModelState is invalid. Errors: {@Errors}",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());

            ViewBag.Categories = _unitOfWork._categoryRopo.getAll();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var artisan = _unitOfWork._artisanRepo.getById(id);
            if (artisan != null)
            {
                artisan.IdNavigation.IsDeleted = true;
                _unitOfWork._artisanRepo.edit(artisan);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }
    }
}