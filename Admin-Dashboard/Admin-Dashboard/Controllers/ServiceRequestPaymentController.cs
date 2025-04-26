using Admin_Dashboard.Enums;
using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Admin_Dashboard.Controllers
{
    public class ServiceRequestPaymentController : Controller
    {
        private readonly ILogger<ServiceRequestPaymentController> logger;
        private readonly UnitOFWork unitOFWork;
        private readonly Dictionary<string, string>  statusDictionary = new Dictionary<string, string> {
        { "1", "Service Requested" },
        { "2", "In Progress" },
        { "3", "Artisan on the Way" },
        { "4", "Artisan Nearing Location" },
        { "5", "Artisan Arrived" },
        { "6", "Service Undergoing" },
        { "7", "Service Completed" },
        { "8", "Service Cancelled" },
        { "9", "Awaiting Approval" },
        { "10", "Artisan Busy" }
    };

        // Constructor
        public ServiceRequestPaymentController(ILogger<ServiceRequestPaymentController> logger, UnitOFWork unitOFWork)
        {
            this.logger = logger;
            this.unitOFWork = unitOFWork;
        }

        // Index action to show all ServiceRequestPayments
        public IActionResult Index()
        {
            var SRP = unitOFWork._ServiceRequestPaymentRepo.GetAll();
            return View(SRP);
        }

        // Edit action (GET) to load the ServiceRequestPayment details in the view
        public IActionResult Edit(int paymentId, string customerId, int serviceId)
        {
            var SRP = unitOFWork._ServiceRequestPaymentRepo.GetByIDS(customerId, paymentId, serviceId);
            if (SRP == null)
            {
                logger.LogError($"ServiceRequestPayment not found: PaymentId={paymentId}, CustomerId={customerId}, ServiceId={serviceId}");
                return NotFound();
            }

            var cus = unitOFWork._CustomerRepo.GetCustomerById(customerId);

            // Define the dictionary for mapping status codes to their corresponding string values
           

            // Map status to string (if it exists in dictionary, else keep the original value)
            string status = statusDictionary.ContainsKey(SRP.Status) ? statusDictionary[SRP.Status] : SRP.Status;

            var SRP_VM = new ServiceRequestPaymentViewModel()
            {
                CustomerId = customerId,
                ServiceId = serviceId,
                PaymentId = SRP.PaymentId,
                Status = status,  // Use the mapped status
                Date = SRP.CreatedAt,
                ExecutionTime = SRP.ExecutionTime,
                CustomerName = cus.FName + " " + cus.LName,
                ServiceName = unitOFWork._ServiceRepo.GetById(serviceId).ServiceName,
                PaymentMethod = unitOFWork._PaymentRepo.GetById(paymentId).Method,
                PaymentStatus = unitOFWork._PaymentRepo.GetById(paymentId).Status,
                Amount = unitOFWork._PaymentRepo.GetById(paymentId).Amount,
            };

            return View(SRP_VM);
        }


        // Edit action (POST) to save the updates
        [HttpPost]
        public IActionResult Edit(ServiceRequestPaymentViewModel SRP_VM)
        {

            if (ModelState.IsValid)
            {
                // Map the selected status (numeric) to the corresponding string value
                

                string statusToSave = statusDictionary.ContainsKey(SRP_VM.Status) ? statusDictionary[SRP_VM.Status] : SRP_VM.Status;

                // If "Other" is selected, save custom status
                if (SRP_VM.Status == "Other" && !string.IsNullOrEmpty(SRP_VM.CustomStatus))
                {
                    statusToSave = SRP_VM.CustomStatus;
                }

                var SRP = unitOFWork._ServiceRequestPaymentRepo.GetByIDS(SRP_VM.CustomerId, SRP_VM.PaymentId, SRP_VM.ServiceId);

                // Check if Status is Changed
                if (SRP.Status != statusToSave)
                    //SendNotification();

                if (SRP != null)
                {
                    if (SRP.Status != statusToSave)
                        //SendNotification();

                    SRP.Status = statusToSave;
                    SRP.CreatedAt = SRP_VM.Date;
                    SRP.ExecutionTime = SRP_VM.ExecutionTime;

                    unitOFWork._ServiceRequestPaymentRepo.Edit(SRP);
                    unitOFWork.save();
                }

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "The selected Customer, Payment, or Service could not be found.");
            }
            return View(SRP_VM);
        }

        // Detail action to show the details of a ServiceRequestPayment
        public IActionResult Detail(int paymentId, string customerId, int serviceId)
        {
            var SRP = unitOFWork._ServiceRequestPaymentRepo.GetByIDS(customerId, paymentId, serviceId);
            if (SRP == null)
            {
                logger.LogError($"ServiceRequestPayment not found: PaymentId={paymentId}, CustomerId={customerId}, ServiceId={serviceId}");
                return NotFound();
            }
            var cus = unitOFWork._CustomerRepo.GetCustomerById(customerId);
            var SRP_VM = new ServiceRequestPaymentViewModel()
            {
                CustomerId=customerId,
                ServiceId=serviceId,
                PaymentId=SRP.PaymentId,
                Status = SRP.Status,
                Date = SRP.CreatedAt,
                ExecutionTime = SRP.ExecutionTime,
                CustomerName = cus.FName+" "+cus.LName,
                ServiceName = unitOFWork._ServiceRepo.GetById(serviceId).ServiceName,
                PaymentMethod = unitOFWork._PaymentRepo.GetById(paymentId).Method,
                PaymentStatus = unitOFWork._PaymentRepo.GetById(paymentId).Status,
                Amount = unitOFWork._PaymentRepo.GetById(paymentId).Amount,
            };

            return View(SRP_VM);
        }
    }
}
