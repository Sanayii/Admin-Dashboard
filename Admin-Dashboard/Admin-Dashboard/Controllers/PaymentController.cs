using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> logger;
        private readonly UnitOFWork unitOFWork;
        public PaymentController(ILogger<PaymentController> logger,UnitOFWork unitOFWork)
        {
            this.logger = logger;
            this.unitOFWork = unitOFWork;
        }
        public IActionResult Index()
        {
            var payments=unitOFWork._PaymentRepo.GetAll();
            return View(payments);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                unitOFWork._PaymentRepo.Add(payment);
                unitOFWork.save();
                return RedirectToAction("Index");
            }
            return View(payment);
        }
        public IActionResult Edit(int id)
        {
            var payment = unitOFWork._PaymentRepo.GetById(id);
            return View(payment);
        }
        [HttpPost]
        public IActionResult Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                unitOFWork._PaymentRepo.Edit(payment);
                unitOFWork.save();
                return RedirectToAction("Index");
            }
            return View(payment);
        }
        public IActionResult Delete(int id)
        {
            unitOFWork._PaymentRepo.Delete(id);
            unitOFWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int id)
        {
            var payment = unitOFWork._PaymentRepo.GetById(id);
            return View(payment);
        }
    }
}
