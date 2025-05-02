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
        [HttpGet]
        public IActionResult Edit(string customerId, int discountId)
        {
            if (customerId == null || discountId == 0)
                return NotFound();

            var customerDiscount = Unit._CustomerDiscountRepo.GetCustomerDiscount(customerId, discountId);
            if (customerDiscount == null)
                return NotFound();

            ViewBag.discounts = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name");
            ViewBag.customers = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "UserName");

            return View(customerDiscount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string originalCustomerId, int originalDiscountId, [Bind("CustomerId,DiscountId,DateGiven")] CustomerDiscount newCustomerDiscount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Unit._CustomerDiscountRepo.ReplaceCustomerDiscount(originalCustomerId, originalDiscountId, newCustomerDiscount);
                    Unit.save();
                }
                catch (Exception ex)
                {
                    return BadRequest("Errore accured During Editing!" + ex.Message);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.discounts = new SelectList(Unit._DiscountRepo.GetAll(), "Id", "Name", newCustomerDiscount.DiscountId);
            ViewBag.customers = new SelectList(Unit._CustomerRepo.GetAll(), "Id", "UserName", newCustomerDiscount.CustomerId);

            return View(newCustomerDiscount);
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