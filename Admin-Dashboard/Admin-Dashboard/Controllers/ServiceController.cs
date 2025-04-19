using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Dashboard.Controllers
{
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
            var services = Unit._serviceRopo.getAll(); 
            var categories = Unit._categoryRopo.getAll().ToDictionary(c => c.Id, c => c.Name); 

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
            var categories = Unit._categoryRopo.getAll().Select(c => new
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
                Unit._serviceRopo.add(service);
                Unit.save();
                return RedirectToAction("Index");
            }
            return View(service);
        }
        public IActionResult Edit(int id)
        {
            var service = Unit._serviceRopo.getById(id);
            if (service == null)
            {
                return NotFound();
            }
            var categories = Unit._categoryRopo.getAll().Select(c => new
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
                Unit._serviceRopo.edit(service);
                Unit.save();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(Unit._categoryRopo.getAll(), "Id", "Name", service.CategoryId);
            return View(service);
        }
        public IActionResult Delete(int id)
        {
            var service = Unit._serviceRopo.getById(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var service = Unit._serviceRopo.getById(id);
            if (service == null)
            {
                return NotFound();
            }
            Unit._serviceRopo.delete(service);
            Unit.save();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var service = Unit._serviceRopo.getById(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
    }
}
