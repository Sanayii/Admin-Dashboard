using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.Areas.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        public UserManager<AppUser> _userManager;

        public CustomerController(UnitOFWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var customers = _unitOfWork._customerRepo.GetAllCustomers();
            return View(customers);
        }
        public IActionResult GetAllCustomers()
        {
            var AllCustomers = _unitOfWork._customerRepo.getAll();
            return View("Index", AllCustomers);
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var customer = _unitOfWork._customerRepo.getById(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var existingCustomer = _unitOfWork._customerRepo.getById(customer.Id);
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
            var customer = _unitOfWork._customerRepo.getById(id);
            if (customer != null)
            {

                customer.IsDeleted = true;
                _unitOfWork._customerRepo.edit(customer);
                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }

    }
}
