using Admin_Dashboard.Models;
using Admin_Dashboard.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard.Controllers
{
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
            var payments=unitOFWork._paymentRopo.getAll();
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
                unitOFWork._paymentRopo.add(payment);
                unitOFWork.save();
                return RedirectToAction("Index");
            }
            return View(payment);
        }
        public IActionResult Edit(int id)
        {
            var payment = unitOFWork._paymentRopo.getById(id);
            return View(payment);
        }
        [HttpPost]
        public IActionResult Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                unitOFWork._paymentRopo.edit(payment);
                unitOFWork.save();
                return RedirectToAction("Index");
            }
            return View(payment);
        }
        public IActionResult Delete(int id)
        {
            unitOFWork._paymentRopo.delete(id);
            unitOFWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int id)
        {
            var payment = unitOFWork._paymentRopo.getById(id);
            return View(payment);
        }
    }
}
