using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;

namespace Admin_Dashboard.Controllers
{
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
            var allCustomerDiscounts = Unit._CustomerDiscountRepo.GetAll().ToList();
            return View(allCustomerDiscounts);
        }

        // GET: CustomerDiscounts/Details/5
        public IActionResult Details(string customerId, int discountId)
        {

            if (string.IsNullOrEmpty(customerId))
            {
                return NotFound();
            }

            var customerDiscount = Unit._CustomerDiscountRepo
                .GetAll()
                .FirstOrDefault(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);
            if (customerDiscount == null)
            {
                return NotFound();
            }

            customerDiscount.Customer = Unit._CustomerRepo.GetById(customerId);
            customerDiscount.Discount = Unit._DiscountRepo.GetById(discountId);

            return View(customerDiscount);


        }

        //// GET: CustomerDiscounts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "Id");
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

            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name", customerDiscount.DiscountId);
            return View(customerDiscount);
        }

        //// GET: CustomerDiscounts/Edit/5
        public IActionResult Edit(string customerId, int discountId)
        {
            if (customerId == null || discountId == 0)
                return NotFound();

            // استخدم Unit of Work
            var customerDiscount = Unit._CustomerDiscountRepo
                .GetAll()
                .FirstOrDefault(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);

            if (customerDiscount == null)
                return NotFound();

            // ملأ ViewData للمحتوى المنسدِل
            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name", customerDiscount.DiscountId);

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
                    bool exists = Unit._CustomerDiscountRepo
                        .GetAll()
                        .Any(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name", customerDiscount.DiscountId);

            return View(customerDiscount);
        }

        //// GET: CustomerDiscounts/Delete/5
        public IActionResult Delete(string customerId, int discountId)
        {
            if (string.IsNullOrEmpty(customerId) || discountId == 0)
            {
                return NotFound();
            }

            var customerDiscount = Unit._CustomerDiscountRepo
                .GetAll()
                .FirstOrDefault(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);

            if (customerDiscount == null)
            {
                return NotFound();
            }

            Unit._CustomerDiscountRepo.Delete(customerDiscount); // نحذف العنصر
            Unit.save(); // نحفظ التغييرات

            return RedirectToAction("Index");
        }

        //// POST: CustomerDiscounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var customerDiscount = await _context.CustomerDiscounts.FindAsync(id);
        //    if (customerDiscount != null)
        //    {
        //        _context.CustomerDiscounts.Remove(customerDiscount);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CustomerDiscountExists(string id)
        //{
        //    return _context.CustomerDiscounts.Any(e => e.CustomerId == id);
        //}
    }
}