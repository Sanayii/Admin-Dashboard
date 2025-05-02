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
        //this method, we can use it in case of if we want to edit date data only... as CustoemerId and DiscountId are part of the primary key
        public void EditCustomerDiscount(CustomerDiscount customerDiscount)
        {
            // Get the existing customer discount using the composite key
            var existing = db.CustomerDiscounts
                .FirstOrDefault(cd => cd.CustomerId == customerDiscount.CustomerId && cd.DiscountId == customerDiscount.DiscountId);

            if (existing != null)
            {
                //update the existing record with the new values of date given...
                existing.DateGiven = customerDiscount.DateGiven;

               
            }
        }

        public void DeleteCustomerDiscount(string customerId, int discountId)
        {
            var customerDiscount = db.CustomerDiscounts.Find(customerId, discountId);
            if (customerDiscount != null)
            {
                db.CustomerDiscounts.Remove(customerDiscount);
            }
        }


        //this method, we can get the customer discount by customerId and discountId
        public CustomerDiscount GetCustomerDiscount(string customerId, int discountId)
        {
            return db.CustomerDiscounts.Include(cd => cd.Customer).Include(cd => cd.Discount)
                .FirstOrDefault(cd => cd.CustomerId == customerId && cd.DiscountId == discountId);
        }


        //this method=> we can use it in edit DiscuntCustomer data totally. firstly we will remove the old data and then add the new one..
        public void ReplaceCustomerDiscount(string oldCustomerId, int oldDiscountId, CustomerDiscount newCustomerDiscount)
        {
            var existing = db.CustomerDiscounts
                .FirstOrDefault(cd => cd.CustomerId == oldCustomerId && cd.DiscountId == oldDiscountId);

            if (existing != null)
            {
                db.CustomerDiscounts.Remove(existing);
            }

            db.CustomerDiscounts.Add(newCustomerDiscount);
        }


    }
}
