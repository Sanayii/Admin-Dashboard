using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Repository;
using Admin_Dashboard.ViewModels;   
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

            if (artisan == null )
                return NotFound();

            var viewModel = new ArtisanViewModel
            {
                Id = artisan.Id,
                Age = artisan.Age,
                FName = artisan.FName,
                LName = artisan.LName,
                Email = artisan.Email,
                City = artisan.City,
                Street = artisan.Street,
                Government = artisan.Government,
                NationalityId = artisan.NationalityId,
                Phones = artisan.UserPhones.Select(p => p.PhoneNumber).ToList(),
                CategoryId = artisan.CategoryId,
                Rating = artisan.Rating

            };
            ViewBag.Categories = _unitOfWork._categoryRopo.getAll();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ArtisanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var artisan = _unitOfWork._artisanRepo.getById(viewModel.Id);

                if (artisan == null)
                    return NotFound();

                artisan.FName = viewModel.FName;
                artisan.LName = viewModel.LName;
                artisan.Age = viewModel.Age;
                artisan.Email = viewModel.Email;
                artisan.City = viewModel.City;
                artisan.Street = viewModel.Street;
                artisan.Government = viewModel.Government;
                artisan.NationalityId = viewModel.NationalityId;
                artisan.CategoryId = viewModel.CategoryId;
                artisan.Rating = viewModel.Rating;

                // Clear old phones and add new ones
                artisan.UserPhones.Clear();
                foreach (var phone in viewModel.Phones)
                {
                    artisan.UserPhones.Add(new UserPhone
                    {
                        PhoneNumber = phone,
                        UserId = artisan.Id
                    });
                }

                _unitOfWork._artisanRepo.edit(artisan); // optional if EF tracks automatically
                _unitOfWork.save();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = _unitOfWork._categoryRopo.getAll();
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            var artisan = _unitOfWork._artisanRepo.getById(id);
            if (artisan != null)
            {
                artisan.IsDeleted = true;
                _unitOfWork._artisanRepo.edit(artisan);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }
    }
}