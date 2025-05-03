using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Repository;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArtisanController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        private readonly ILogger<ArtisanController> _logger;
        public UserManager<AppUser> _userManager;

        public ArtisanController(UnitOFWork unitOfWork, ILogger<ArtisanController> logger, UserManager<AppUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = usermanager;
        }

        //return active artisans only
        public IActionResult Index()
        {
            var artisans = _unitOfWork._ArtisanRepo.GetAllArtisan();
            return View(artisans);
        }

        //return active and unactive artisans
        public IActionResult GetAllArtisans()
        {
            var AllArtisans = _unitOfWork._ArtisanRepo.GetAll();
            return View("Index", AllArtisans);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var artisan = _unitOfWork._ArtisanRepo.GetById(id);

            if (artisan == null)
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

            ViewBag.Categories = _unitOfWork._CategoryRepo.GetAll();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ArtisanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var artisan = _unitOfWork._ArtisanRepo.GetById(viewModel.Id);

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

                _unitOfWork._ArtisanRepo.Edit(artisan); // optional if EF tracks automatically
                _unitOfWork.save();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = _unitOfWork._CategoryRepo.GetAll();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var artisan = _unitOfWork._ArtisanRepo.GetById(id);
            if (artisan != null)
            {
                artisan.IsDeleted = true;
                _unitOfWork._ArtisanRepo.Edit(artisan);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Activate(string id)
        {
            var artisan = _unitOfWork._ArtisanRepo.GetById(id);
            if (artisan != null)
            {
                artisan.IsDeleted = false;
                _unitOfWork._ArtisanRepo.Edit(artisan);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Initialize view model with default values
                var viewModel = new ArtisanViewModel
                {
                    Phones = new List<string> { "" }, // Start with one empty phone field
                    Rating = 3 // Default rating
                };

                // Load categories for dropdown
                ViewBag.Categories = _unitOfWork._CategoryRepo.GetAll();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create artisan form");
                TempData["ErrorMessage"] = "Failed to load create form. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtisanViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _unitOfWork._CategoryRepo.GetAll();
                    return View(model);
                }

                // Create Artisan directly (without creating a separate AppUser)
                var artisan = new Artisan
                {
                    UserName = model.Email,  // Add IdentityUser properties
                    Email = model.Email,
                    FName = model.FName,
                    LName = model.LName,
                    Age = model.Age,
                    City = model.City,
                    Street = model.Street,
                    Government = model.Government,
                    EmailConfirmed = true,
                    NationalityId = model.NationalityId,
                    Rating = model.Rating,
                    CategoryId = model.CategoryId
                };

                // Add phone numbers
                artisan.UserPhones = model.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new UserPhone { PhoneNumber = p })
                    .ToList();

                // Create account with temporary password
                var result = await _userManager.CreateAsync(artisan, "DefaultArtisan@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(artisan, "Artisan");
                    TempData["SuccessMessage"] = "Artisan created successfully!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating artisan");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                Console.WriteLine(ex.ToString());
            }

            ViewBag.Categories = _unitOfWork._CategoryRepo.GetAll();
            return View(model);
        }
        //search by admin name
        [HttpGet]
        public IActionResult Search2(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var allArtisans = _unitOfWork._ArtisanRepo.GetAllArtisan();

            var filteredArtisans = allArtisans
                .Where(a =>
                    (a.IdNavigation.FName != null && a.IdNavigation.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IdNavigation.LName != null && a.IdNavigation.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return View("Index", filteredArtisans);
        }

        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Json(new { success = true, data = new List<object>(), count = 0 });
                }

                var allArtisans = _unitOfWork._ArtisanRepo.GetAllArtisan()
                    .Where(a => !a.IsDeleted)
                    .ToList();

                var filteredArtisans = allArtisans
                    .Where(a =>
                        (a.FName != null && a.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.LName != null && a.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.Email != null && a.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(a => new
                    {
                        id = a.Id,
                        fName = a.FName,
                        lName = a.LName,
                        email = a.Email,
                        age = a.Age,
                        isDeleted = a.IsDeleted,
                        phones = a.UserPhones?.Select(p => p.PhoneNumber).Take(2).ToList(),
                        phoneCount = a.UserPhones?.Count ?? 0,
                        rating = a.Rating,
                        category = a.Category?.Name
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = filteredArtisans,
                    count = filteredArtisans.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching artisans");
                return Json(new { success = false, message = "An error occurred while searching", data = new List<object>() });
            }
        }
    }
}
