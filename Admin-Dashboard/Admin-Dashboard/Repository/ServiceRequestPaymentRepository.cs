using Admin_Dashboard.Models;

namespace Admin_Dashboard.Repository
{
    public class ServiceRequestPaymentRepository:GenericRepository<ServiceRequestPayment>
    {
        public ServiceRequestPaymentRepository(SanayiiContext db) : base(db)
        {
        }
        public ServiceRequestPayment GetByIDS(string customerId, int paymentId, int serviceId)
        {
            return db.ServiceRequestPayments.FirstOrDefault(x => x.CustomerId == customerId && x.PaymentId == paymentId && x.ServiceId == serviceId);
        }
    }
}
