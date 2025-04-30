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
    public class CustomerDiscountsController : Controller
    {
        private readonly UnitOFWork Unit;
        public CustomerDiscountsController(UnitOFWork _Unit)
        {
            Unit = _Unit;
        }

        // GET: CustomerDiscounts
        public IActionResult Index()
        {
            var allCustomerDiscounts = Unit._CustomerDiscountRepo.GetAllCustomerDiscounts();
            return View(allCustomerDiscounts);
        }

        // GET: CustomerDiscounts/Details/5
        public IActionResult Details(string customerId, int discountId)
        {

            if (string.IsNullOrEmpty(customerId))
            {
                return NotFound();
            }

            var customerDiscount = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId);
            if (customerDiscount == null)
            {
                return NotFound();
            }

            return View(customerDiscount);


        }

        //// GET: CustomerDiscounts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "UserName");
            ViewData["DiscountId"] = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CustomerId,DiscountId,DateGiven")] CustomerDiscount customerDiscount)
        {
            if (ModelState.IsValid)
            {
                Unit._CustomerDiscountRepo.Add(customerDiscount);
                Unit.save();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "UserName", customerDiscount.CustomerId);
            var discounts = Unit._DiscountRepo.GetAll();
            ViewBag.discounts = new SelectList(discounts, "Id", "Name");
            return View(customerDiscount);
        }

        //// GET: CustomerDiscounts/Edit/5
        public IActionResult Edit(string customerId, int discountId)
        {
            if (customerId == null || discountId == 0)
                return NotFound();

            var customerDiscount = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId);
            if (customerDiscount == null)
                return NotFound();

            // Fill dropdown and preselect current discount
            var discounts = Unit._DiscountRepo.GetAll();
            ViewBag.discounts = new SelectList(discounts, "Id", "Name");

            return View(customerDiscount);
        }


        // POST: CustomerDiscounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string customerId, int discountId, [Bind("CustomerId,DiscountId,DateGiven")] CustomerDiscount customerDiscount)
        {
            if (customerId != customerDiscount.CustomerId || discountId != customerDiscount.DiscountId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    Unit._CustomerDiscountRepo.Edit(customerDiscount);
                    Unit.save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool exists = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId) != null;

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "Id", customerDiscount.CustomerId);
            var discounts = Unit._DiscountRepo.GetAll();
            ViewBag.discounts = new SelectList(discounts, "Id", "Name");
            return View(customerDiscount);
        }

        //// GET: CustomerDiscounts/Delete/5
        public IActionResult Delete(string customerId, int discountId)
        {
            if (string.IsNullOrEmpty(customerId) || discountId == 0)
            {
                return NotFound();
            }

            var customerDiscount = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId);

            if (customerDiscount == null)
            {
                return NotFound();
            }
            return View(customerDiscount);
        }

        // POST: CustomerDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string customerId, int discountId)
        {
            var customerDiscount = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId);

            if (customerDiscount != null)
            {
                Unit._CustomerDiscountRepo.DeleteCustomerDiscount(customerId,discountId);
            }

            Unit.save();
            return RedirectToAction(nameof(Index));
        }
    }
}