using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var contracts = Unit._ContractRepo.GetAll();

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
            var artisans = Unit._ArtisanRepo.GetAll()
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
            var artisans = Unit._ArtisanRepo.GetAll()
                          .Select(a => new {
                              Id = a.Id,
                              Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                          }).ToList();

            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after Start date.");
                ViewBag.Artisans = new SelectList(artisans, "Id", "Name");
                return View(contract);
            }


            if (ModelState.IsValid)
            {
                Unit._ContractRepo.Add(contract);
                Unit.save();
                return RedirectToAction("Index");
            }


            
            ViewBag.Artisans = new SelectList(artisans, "Id", "Name");
            return View(contract);
        }

        public IActionResult Edit(int id)
        {

            var contract = Unit._ContractRepo.GetById(id);
            if (contract == null)
            {
                return NotFound();
            }

            var artisans = Unit._ArtisanRepo.GetAll()
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
            var artisans = Unit._ArtisanRepo.GetAll()
                          .Select(a => new {
                              Id = a.Id,
                              Name = $"{a.IdNavigation?.FName} {a.IdNavigation?.LName}"
                          }).ToList();

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name", contract.ArtisanId);
            if (id != contract.Id)
            {
                return NotFound();
            }
            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after Start date.");

                return View(contract);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Unit._ContractRepo.Edit(contract);
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

            ViewBag.Artisans = new SelectList(artisans, "Id", "Name", contract.ArtisanId);
            return View(contract);
        }

        private bool ContractExists(int id)
        {
            return Unit._ContractRepo.GetAll().Any(e => e.Id == id);
        }
        public IActionResult Delete(int id)
        {
            Unit._ContractRepo.Delete(id);
            Unit.save();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {

            var contract = Unit._ContractRepo.GetById(id);

            if (contract == null) return NotFound();

            if (!string.IsNullOrEmpty(contract.ArtisanId))
            {
                var artisan = Unit._ArtisanRepo.GetById(contract.ArtisanId);
                if (artisan?.IdNavigation != null)
                {
                    ViewBag.ArtisanName = $"{artisan.IdNavigation.FName} {artisan.IdNavigation.LName}";
                }
            }

            return View(contract);

        }

    }
}