using Microsoft.AspNetCore.Mvc;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Authorization;
namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        UnitOFWork UnitOfWorks;
        public CategoryController(UnitOFWork _unitofWorks) { 
            UnitOfWorks = _unitofWorks;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult All()
        {
            var Categorys = UnitOfWorks._CategoryRepo.GetAll(); 
            return View(Categorys);
        }
        public ActionResult Detail(int id)
        {
            if (id == null) return BadRequest();
            var item = UnitOfWorks._CategoryRepo.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }
        public IActionResult Delete(int id)
        {
            if (id == null) return BadRequest();
            var item = UnitOfWorks._CategoryRepo.GetById(id);
            if (item == null) return NotFound();
            try
            {
                UnitOfWorks._CategoryRepo.Delete(id);
                UnitOfWorks.save();
            }
            catch (DbUpdateException ex)
            {
                ViewBag.ErrorMessage = "Cannot delete this category.";
                return View();
            }

            return RedirectToAction("All");
        }
        [HttpPost]
        public IActionResult Add(Category catg)
        {
            if (!ModelState.IsValid)
            {
                return View(catg);
            }

            UnitOfWorks._CategoryRepo.Add(catg);
            UnitOfWorks.save();
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Edit(Category catg)
        {
            if (!ModelState.IsValid)
            {
                return View(catg);
            }

            UnitOfWorks._CategoryRepo.Edit(catg);
            UnitOfWorks.save();
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null) return BadRequest();
            var catg = UnitOfWorks._CategoryRepo.GetById(id);
            if (catg == null) return NotFound();
            return View(catg);
        }
    }
}
