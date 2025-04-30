using Admin_Dashboard.Enums;
using Admin_Dashboard.Models;
using Admin_Dashboard.Repository;
using Admin_Dashboard.Services;
using Admin_Dashboard.UnitOfWorks;
using Admin_Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Admin_Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceRequestPaymentController : Controller
    {
        private readonly ILogger<ServiceRequestPaymentController> logger;
        private readonly UnitOFWork unitOFWork;
        private readonly INotificationService notificationService;
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
        public ServiceRequestPaymentController(ILogger<ServiceRequestPaymentController> logger, UnitOFWork unitOFWork,INotificationService notificationService)
        {
            this.logger = logger;
            this.unitOFWork = unitOFWork;
            this.notificationService = notificationService;
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


            var SRP_VM = new ServiceRequestPaymentViewModel()
            {
                CustomerId = customerId,
                ServiceId = serviceId,
                PaymentId = SRP.PaymentId,
                Status = SRP.Status,  // Use the mapped status
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
        public async Task<IActionResult> Edit(ServiceRequestPaymentViewModel SRP_VM)
        {

            if (ModelState.IsValid)
            {
                var SRP = unitOFWork._ServiceRequestPaymentRepo.GetByIDS(SRP_VM.CustomerId, SRP_VM.PaymentId, SRP_VM.ServiceId);


                if (SRP != null)
                {
                    if (SRP.Status != SRP_VM.Status)
                        {
                            var notification = new Notification
                            {
                                UserId = SRP.CustomerId,
                                Title = "Service Request Status Update",
                                Content = $"Your service request status has been updated to: {SRP_VM.Status}",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            };
                            await notificationService.SendNotification(notification);
                        }
                    SRP.Status = SRP_VM.Status;
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
