using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UnitOFWork _unitOfWork;

        public CustomerController(UnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var customers = _unitOfWork._customerRepo.getAll();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var customer = _unitOfWork._customerRepo.getById(id);
            if (customer == null || customer.IdNavigation == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork._customerRepo.edit(customer);
                _unitOfWork._customerRepo.db.Users.Update(customer.IdNavigation);

                foreach (var phone in customer.IdNavigation.UserPhones)
                {
                    _unitOfWork._customerRepo.db.UserPhones.Update(phone);
                }

                _unitOfWork.save();
                return RedirectToAction("Index");
            }

            return View(customer);
        }
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var customer = _unitOfWork._customerRepo.getById(id);
            if (customer != null)
            {
                // Soft delete: Mark the user as deleted
                customer.IdNavigation.IsDeleted = true;

                _unitOfWork._customerRepo.edit(customer);
                _unitOfWork._customerRepo.db.Users.Update(customer.IdNavigation);

                _unitOfWork.save();
            }

            return RedirectToAction("Index");
        }

    }
}
