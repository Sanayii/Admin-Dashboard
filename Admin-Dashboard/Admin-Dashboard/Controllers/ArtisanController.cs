using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Repository;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Identity;
namespace Admin_Dashboard.Controllers
{
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

        public IActionResult Index()
        {
            var artisans = _unitOfWork._artisanRepo.getAllArtisan();
            return View(artisans);
        }
        public IActionResult GetAllArtisans()
        {
            var AllArtisans = _unitOfWork._artisanRepo.getAll();
            return View("Index", AllArtisans);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var artisan = _unitOfWork._artisanRepo.getById(id);

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
                ViewBag.Categories = _unitOfWork._categoryRopo.getAll();

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
                    ViewBag.Categories =  _unitOfWork._categoryRopo.getAll();
                    return View(model);
                }

                // إنشاء Artisan مباشرةً (بدون إنشاء AppUser منفصل)
                var artisan = new Artisan
                {
                    UserName = model.Email,  // إضافة خصائص IdentityUser
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

                // إضافة أرقام الهواتف
                artisan.UserPhones = model.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new UserPhone { PhoneNumber = p })
                    .ToList();

                // إنشاء الحساب بكلمة مرور مؤقتة
                var result = await _userManager.CreateAsync(artisan, "DefaultArtisan@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(artisan, "Artisan");
                    TempData["SuccessMessage"] = "تم إنشاء الحرفي بنجاح!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إنشاء الحرفي");
                ModelState.AddModelError("", "حدث خطأ غير متوقع. الرجاء المحاولة مرة أخرى.");
                Console.WriteLine(ex.ToString());
                
            }

            ViewBag.Categories = _unitOfWork._categoryRopo.getAll();
            return View(model);
        }

    }
}