using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DiscountsController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly UnitOFWork _unitOfWork;


        public DiscountsController(
            ILogger<AdminController> logger,
            UnitOFWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        // GET: Discounts
        public IActionResult Index()
        {
            var discounts = _unitOfWork._DiscountRepo.GetAll();
            return View(discounts);
        }

        // GET: Discounts/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = _unitOfWork._DiscountRepo.GetById(id);

            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // GET: Discounts/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Amount,MinRequiredRequests,IsFixedAmount,IsPercentage,ExpireDate")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork._DiscountRepo.Add(discount);
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // GET: Discounts/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = _unitOfWork._DiscountRepo.GetById(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Amount,MinRequiredRequests,IsFixedAmount,IsPercentage,ExpireDate")] Discount discount)
        {
            if (id != discount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork._DiscountRepo.Edit(discount);
                    _unitOfWork.save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.Id))
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
            return View(discount);
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = _unitOfWork._DiscountRepo.GetById(id);

            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var discount = _unitOfWork._DiscountRepo.GetById(id);
            if (discount != null)
            {
                _unitOfWork._DiscountRepo.Delete(discount.Id);
                _unitOfWork.save();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _unitOfWork._DiscountRepo.GetAll().Any(e => e.Id == id);
        }
    }
}