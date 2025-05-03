using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.Areas.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        public UserManager<AppUser> _userManager;
        public ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger,UnitOFWork unitOfWork, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        //return active customers only..
        public IActionResult Index()
        {
            var customers = _unitOfWork._CustomerRepo.GetAllCustomers();
            return View(customers);
        }
        //return All customers
        public IActionResult GetAllCustomers()
        {
            var AllCustomers = _unitOfWork._CustomerRepo.GetAll();
            return View("Index", AllCustomers);
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var customer = _unitOfWork._CustomerRepo.GetById(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var existingCustomer = _unitOfWork._CustomerRepo.GetById(customer.Id);
            if (existingCustomer == null)
                return NotFound();

            existingCustomer.FName = customer.FName;
            existingCustomer.LName = customer.LName;
            existingCustomer.Age = customer.Age;
            existingCustomer.Email = customer.Email;
            existingCustomer.City = customer.City;
            existingCustomer.Street = customer.Street;
            existingCustomer.Government = customer.Government;
            existingCustomer.UserPhones.Clear();    
            existingCustomer.UserPhones = customer.UserPhones;

            _unitOfWork.save();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            var customer = _unitOfWork._CustomerRepo.GetById(id);
            if (customer != null)
            {

                customer.IsDeleted = true;
                _unitOfWork._CustomerRepo.Edit(customer);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Activate(string id)
        {
            var customer = _unitOfWork._CustomerRepo.GetById(id);
            if (customer != null)
            {

                customer.IsDeleted = false;
                _unitOfWork._CustomerRepo.Edit(customer);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }

        //search by csutomer name,email..
        [HttpGet]
        public IActionResult Search2(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var allCustomers = _unitOfWork._CustomerRepo.GetAllCustomers();

            var filteredCustomers = allCustomers
                .Where(a =>
                    (a.IdNavigation.FName != null && a.IdNavigation.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IdNavigation.LName != null && a.IdNavigation.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IdNavigation.Email !=null && a.IdNavigation.Email.Contains(searchTerm,StringComparison.OrdinalIgnoreCase))||
                    (a.IdNavigation.Government !=null && a.IdNavigation.Government.Contains(searchTerm,StringComparison.OrdinalIgnoreCase))||
                     (a.IdNavigation.City != null && a.IdNavigation.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IdNavigation.Street != null && a.IdNavigation.Street.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))


                .ToList();

            return View("Index", filteredCustomers);
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

                var allCustomers = _unitOfWork._CustomerRepo.GetAllCustomers()
                    .Where(c => !c.IsDeleted)
                    .ToList();

                var filteredCustomers = allCustomers
                    .Where(c =>
                        (c.FName != null && c.FName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.LName != null && c.LName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.Email != null && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.Government != null && c.Government.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.City != null && c.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.Street != null && c.Street.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(c => new
                    {
                        id = c.Id,
                        fName = c.FName,
                        lName = c.LName,
                        email = c.Email,
                        age = c.Age,
                        city = c.City,
                        government = c.Government,
                        street = c.Street,
                        isDeleted = c.IsDeleted,
                        phones = c.UserPhones?.Select(p => p.PhoneNumber).Take(2).ToList(),
                        phoneCount = c.UserPhones?.Count ?? 0
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = filteredCustomers,
                    count = filteredCustomers.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers");
                return Json(new { success = false, message = "An error occurred while searching", data = new List<object>() });
            }
        }

    }
}
