using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class CustomerDiscountRepository : GenericRepository<CustomerDiscount>
    {
        public CustomerDiscountRepository(SanayiiContext db) : base(db)
        {
        }
        public List<CustomerDiscount> GetAllCustomerDiscounts()
        {
            return db.CustomerDiscounts.Include(cd => cd.Customer).Include(cd => cd.Discount).ToList();
        }
        public void AddCustomerDiscount(CustomerDiscount customerDiscount)
        {
            db.CustomerDiscounts.Add(customerDiscount);
        }
        public void EditCustomerDiscount(CustomerDiscount customerDiscount)
        {
            db.CustomerDiscounts.Update(customerDiscount);
        }
        public void DeleteCustomerDiscount(string customerId, int discountId)
        {
            var customerDiscount = db.CustomerDiscounts.Find(customerId, discountId);
            if (customerDiscount != null)
            {
                db.CustomerDiscounts.Remove(customerDiscount);
            }
        }
        public CustomerDiscount GetCustomerDiscount(string customerId, int discountId)
        {
            return db.CustomerDiscounts.Include(cd => cd.Customer).Include(cd => cd.Discount)
                .FirstOrDefault(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);
        }

    }
}
