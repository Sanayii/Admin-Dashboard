using Admin_Dashboard.Enums;
using Admin_Dashboard.Models;

namespace Admin_Dashboard.ViewModels
{
    public class ServiceRequestPaymentViewModel
    {
        public string CustomerId { get; set; }
        public int PaymentId { get; set; }
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }
        public string CustomerName { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int ExecutionTime { get; set; }

    }
}
