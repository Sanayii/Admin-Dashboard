using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ViolationsController : Controller
    {
        private readonly SanayiiContext _context;
        private UnitOFWork UnitOfWorks;
        public ViolationsController(SanayiiContext context, UnitOFWork _unitOFWork)
        {
            _context = context;
            UnitOfWorks = _unitOFWork;
        }

        // GET: Violations
        public IActionResult Index()
        {
            var s = UnitOfWorks._ViolationRepo.GetAll();
            return View(s);
        }

        // GET: Violations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var violation = UnitOfWorks._ViolationRepo.GetById(id);
            if (violation == null)
            {
                return NotFound();
            }

            return View(violation);
        }

        // GET: Violations/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id");
            return View();
        }

        // POST: Violations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reason,Status,Date,ContractId")] Violation violation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(violation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ArtisanId", violation.ContractId);
            return View(violation);
        }

        // GET: Violations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var violation = await _context.Violations.FindAsync(id);
            if (violation == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ArtisanId", violation.ContractId);
            return View(violation);
        }

        // POST: Violations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reason,Status,Date,ContractId")] Violation violation)
        {
            if (id != violation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(violation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViolationExists(violation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ArtisanId", violation.ContractId);
            return View(violation);
        }
        public IActionResult Delete(int id)
        {
            if (id == null) return BadRequest();
            var item = UnitOfWorks._ViolationRepo.GetById(id);
            if (item == null) return NotFound();
            try
            {
                UnitOfWorks._ViolationRepo.Delete(id);
                UnitOfWorks.save();
            }
            catch (DbUpdateException ex)
            {
                ViewBag.ErrorMessage = "Cannot delete this Violation.";
                return View();
            }

            return RedirectToAction("index");
        }

        private bool ViolationExists(int id)
        {
            return _context.Violations.Any(e => e.Id == id);
        }
    }
}