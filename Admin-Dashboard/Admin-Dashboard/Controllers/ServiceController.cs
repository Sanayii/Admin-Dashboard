using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly UnitOFWork Unit;
        public ServiceController(UnitOFWork unit)
        {
            Unit = unit;
        }
        public IActionResult Index()
        {
            //var services = Unit._serviceRopo.getAll();
            //return View(services);
            var services = Unit._ServiceRepo.GetAll();
            var categories = Unit._CategoryRepo.GetAll().ToDictionary(c => c.Id, c => c.Name);

            var viewModel = services.Select(s => new ServiceViewModel
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                CategoryName = categories.ContainsKey(s.CategoryId) ? categories[s.CategoryId] : "N/A",
                Description = s.Description,
                BasePrice = s.BasePrice,
                AdditionalPrice = s.AdditionalPrice
            }).ToList();

            return View(viewModel);
        }
        public IActionResult Create()
        {
            var categories = Unit._CategoryRepo.GetAll().Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                Unit._ServiceRepo.Add(service);
                Unit.save();
                return RedirectToAction("Index");
            }
            return View(service);
        }
        public IActionResult Edit(int id)
        {
            var service = Unit._ServiceRepo.GetById(id);
            if (service == null)
            {
                return NotFound();
            }
            var categories = Unit._CategoryRepo.GetAll().Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", service.CategoryId);
            return View(service);
        }
        [HttpPost]
        public IActionResult Edit(int id, Service service)
        {
            if (id != service.Id) return NotFound();
            if (ModelState.IsValid)
            {
                Unit._ServiceRepo.Edit(service);
                Unit.save();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(Unit._CategoryRepo.GetAll(), "Id", "Name", service.CategoryId);
            return View(service);
        }
        public IActionResult Delete(int id)
        {
            var service = Unit._ServiceRepo.GetById(id);
            if (service == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var service = Unit._ServiceRepo.GetById(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewBag.Category = service.Category.Name;
            return View(service);
        }
    }
}
