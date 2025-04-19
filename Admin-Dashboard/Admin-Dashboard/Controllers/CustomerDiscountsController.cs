using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin_Dashboard.Models;

namespace Admin_Dashboard.Controllers
{
    public class CustomerDiscountsController : Controller
    {
        private readonly SanayiiContext _context;

        public CustomerDiscountsController(SanayiiContext context)
        {
            _context = context;
        }

        // GET: CustomerDiscounts
        public async Task<IActionResult> Index()
        {
            var sanayiiContext = _context.CustomerDiscounts.Include(c => c.Customer).Include(c => c.Discount);
            return View(await sanayiiContext.ToListAsync());
        }

        // GET: CustomerDiscounts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDiscount = await _context.CustomerDiscounts
                .Include(c => c.Customer)
                .Include(c => c.Discount)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerDiscount == null)
            {
                return NotFound();
            }

            return View(customerDiscount);
        }

        // GET: CustomerDiscounts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Name");
            return View();
        }

        // POST: CustomerDiscounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,DiscountId,DateGiven")] CustomerDiscount customerDiscount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerDiscount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Name", customerDiscount.DiscountId);
            return View(customerDiscount);
        }

        // GET: CustomerDiscounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDiscount = await _context.CustomerDiscounts.FindAsync(id);
            if (customerDiscount == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Name", customerDiscount.DiscountId);
            return View(customerDiscount);
        }

        // POST: CustomerDiscounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,DiscountId,DateGiven")] CustomerDiscount customerDiscount)
        {
            if (id != customerDiscount.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerDiscount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerDiscountExists(customerDiscount.CustomerId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", customerDiscount.CustomerId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Name", customerDiscount.DiscountId);
            return View(customerDiscount);
        }

        // GET: CustomerDiscounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDiscount = await _context.CustomerDiscounts
                .Include(c => c.Customer)
                .Include(c => c.Discount)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerDiscount == null)
            {
                return NotFound();
            }

            return View(customerDiscount);
        }

        // POST: CustomerDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customerDiscount = await _context.CustomerDiscounts.FindAsync(id);
            if (customerDiscount != null)
            {
                _context.CustomerDiscounts.Remove(customerDiscount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerDiscountExists(string id)
        {
            return _context.CustomerDiscounts.Any(e => e.CustomerId == id);
        }
    }
}
