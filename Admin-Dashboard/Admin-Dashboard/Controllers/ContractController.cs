using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Admin_Dashboard.Controllers
{
    public class ContractController : Controller
    {
        private readonly UnitOFWork Unit;
        
        public ContractController(UnitOFWork unit)
        {
            Unit = unit;
            
        }
        public IActionResult Index()
        {
            //var contracts = Unit._contractRepo.getAll();
            //return View(contracts);
            var contracts = Unit._contractRepo.getAll();

            var viewModel = contracts.Select(c => new ContractViewModel
            {
                Id = c.Id,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                MaxViolationsAllowed = c.MaxViolationsAllowed,
                Status = c.Status,
                ArtisanFullName = c.Artisan?.IdNavigation?.FName + " " + c.Artisan?.IdNavigation?.LName ?? "N/A" 
            }).ToList(); 

            return View(viewModel);

        }
        public IActionResult Create()
        {
            var artisans = Unit._artisanRepo.getAll()
                  .Select(a => new {
                      Id = a.Id,
                      Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                  }).ToList();

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Contract contract)
        {
            if (ModelState.IsValid)
            {
                Unit._contractRepo.add(contract);
                Unit.save();
                return RedirectToAction("Index");
            }

            
            var artisans = Unit._artisanRepo.getAll()
                          .Select(a => new {
                              Id = a.Id,
                              Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                          }).ToList();

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name");
            return View(contract);
        }
        //public IActionResult Edit(int id)
        //{
        //    var contract = Unit._contractRepo.getById(id);
        //    if (contract == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(contract);
        //}
        //[HttpPost]
        //public IActionResult Edit(int id, Contract contract)
        //{
        //    if (id != contract.Id) return NotFound();
        //    if (ModelState.IsValid)
        //    {
        //        Unit._contractRepo.edit(contract);
        //        Unit.save();
        //        return RedirectToAction("Index");
        //    }
        //    return View(contract);
        //}
        public IActionResult Edit(int id)
        {
            var contract = Unit._contractRepo.getById(id);
            if (contract == null)
            {
                return NotFound();
            }

            var artisans = Unit._artisanRepo.getAll()
                          .Select(a => new {
                              Id = a.Id,
                              Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                          }).ToList();

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name", contract.ArtisanId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Unit._contractRepo.edit(contract);
                    Unit.save();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }


            var artisans = Unit._artisanRepo.getAll()
                          .Select(a => new {
                              Id = a.Id,
                              Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                          }).ToList();

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name", contract.ArtisanId);
            return View(contract);
        }

        private bool ContractExists(int id)
        {
            return Unit._contractRepo.getAll().Any(e => e.Id == id);
        }
        public IActionResult Delete(int id)
        {
            Unit._contractRepo.delete(id);
            Unit.save();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            //var contract = Unit._contractRepo.getById(id);

            //if (contract == null)
            //{
            //    return NotFound(); 
            //}

            //if (contract.Artisan?.IdNavigation == null)
            //{
            //    ViewBag.ErrorMessage = "Artisan data is missing";
            //    return View(contract); 
            //}

            //return View(contract);
            //var contract = Unit._contractRepo.getById(id);

            //if (contract == null)
            //{
            //    return NotFound();
            //}

            //if (contract.Artisan?.IdNavigation != null)
            //{
            //    ViewBag.ArtisanName = $"{contract.Artisan.IdNavigation.FName} {contract.Artisan.IdNavigation.LName}";
            //}
            //else
            //{
            //    ViewBag.ArtisanName = "N/A";
            //}

            //return View(contract);
            var contract = Unit._contractRepo.getById(id);

            if (contract == null) return NotFound();

            if (!string.IsNullOrEmpty(contract.ArtisanId))
            {
                var artisan = Unit._artisanRepo.getById(contract.ArtisanId);
                if (artisan?.IdNavigation != null)
                {
                    ViewBag.ArtisanName = $"{artisan.IdNavigation.FName} {artisan.IdNavigation.LName}";
                }
            }

            return View(contract);

        }

    }
}