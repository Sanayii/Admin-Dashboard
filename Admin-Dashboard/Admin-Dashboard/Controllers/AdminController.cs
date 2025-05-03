using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UnitOFWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(
            ILogger<AdminController> logger,
            UnitOFWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        
        //return all admins active only
        public IActionResult Index()
        {
            var admins = _unitOfWork._AdminRepo.GetAllAdmins();
            return View(admins);
        }
        //return all admins only
        public IActionResult GetAllAdmins()
        {
            var allAdmins = _unitOfWork._AdminRepo.GetAll();
            return View("Index", allAdmins);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminViewModel vm)
        {
            if (ModelState.IsValid)
            {

                var admin = new Admin
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    FName = vm.FName,
                    LName = vm.LName,
                    Age = vm.Age,
                    City = vm.City,
                    Street = vm.Street,
                    Government = vm.Government,
                    Salary = vm.Salary
                };

                var result = await _userManager.CreateAsync(admin, "DefaultPassword@123");

                if (result.Succeeded)
                {

                    foreach (var phone in vm.Phones)
                    {
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            _unitOfWork._UserPhoneRepo.Add(new UserPhone
                            {
                                PhoneNumber = phone,
                                UserId = admin.Id
                            });
                        }
                    }

                    _unitOfWork.save();

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(vm);
        }



        [HttpGet]
        public IActionResult Edit(string id)
        {
            var admin = _unitOfWork._AdminRepo.GetById(id);

            if (admin == null)
                return NotFound();

            var viewModel = new AdminViewModel
            {
                Id = admin.Id,
                FName = admin.IdNavigation.FName,
                LName = admin.IdNavigation.LName,
                Age = admin.IdNavigation.Age,
                Email = admin.IdNavigation.Email,
                City = admin.IdNavigation.City,
                Street = admin.IdNavigation.Street,
                Government = admin.IdNavigation.Government,
                Phones = admin.IdNavigation.UserPhones.Select(p => p.PhoneNumber).ToList(),
                Salary = admin.Salary
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork._AdminRepo.GetById(viewModel.Id);

                if (admin == null)
                    return NotFound();

                // update Appuser 
                var appUser = admin.IdNavigation;
                appUser.FName = viewModel.FName;
                appUser.LName = viewModel.LName;
                appUser.Age = viewModel.Age;
                appUser.Email = viewModel.Email;
                appUser.City = viewModel.City;
                appUser.Street = viewModel.Street;
                appUser.Government = viewModel.Government;

                //update admin data
                admin.Salary = viewModel.Salary;

                //update phones
                appUser.UserPhones.Clear();
                foreach (var phone in viewModel.Phones)
                {
                    appUser.UserPhones.Add(new UserPhone
                    {
                        PhoneNumber = phone,
                        UserId = appUser.Id
                    });
                }

                await _userManager.UpdateAsync(appUser);
                _unitOfWork._AdminRepo.Edit(admin);
                _unitOfWork.save();

                return RedirectToAction("Index");
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Validation Error: {error.ErrorMessage}");
            }

            return View(viewModel);
        }

        [HttpPost]//deactive admin 
        public IActionResult Delete(string id)
        {
            var admin = _unitOfWork._AdminRepo.GetById(id);

            if (admin != null)
            {
                admin.IdNavigation.IsDeleted = true;
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }

        //activate 
        public IActionResult Activate (string id)
        {
            var admin = _unitOfWork._AdminRepo.GetById(id);
            if (admin != null)
            {
                admin.IdNavigation.IsDeleted = false;
                _unitOfWork.save();
            }

            return RedirectToAction("Index");

        }

    //search by admin name
   [HttpGet]
        public IActionResult Search2(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var allAdmins = _unitOfWork._AdminRepo.GetAllAdmins();

            var filteredAdmins = allAdmins
                .Where(a =>
                    (a.IdNavigation.FName != null && a.IdNavigation.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IdNavigation.LName != null && a.IdNavigation.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return View("Index", filteredAdmins);
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

                var allAdmins = _unitOfWork._AdminRepo.GetAllAdmins()
                    .Where(a => a.IdNavigation != null && !a.IdNavigation.IsDeleted)
                    .ToList();

                var filteredAdmins = allAdmins
                    .Where(a =>
                        (a.IdNavigation.FName != null && a.IdNavigation.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.IdNavigation.LName != null && a.IdNavigation.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (a.IdNavigation.Email != null && a.IdNavigation.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(a => new
                    {
                        id = a.Id,
                        fName = a.IdNavigation.FName,
                        lName = a.IdNavigation.LName,
                        email = a.IdNavigation.Email,
                        salary = a.Salary,
                        age = a.IdNavigation.Age,
                        isDeleted = a.IdNavigation.IsDeleted,
                        phones = a.IdNavigation.UserPhones?.Select(p => p.PhoneNumber).Take(2).ToList(),
                        phoneCount = a.IdNavigation.UserPhones?.Count ?? 0
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = filteredAdmins,
                    count = filteredAdmins.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching admins");
                return Json(new { success = false, message = "An error occurred while searching", data = new List<object>() });
            }
        
    }
    }
}