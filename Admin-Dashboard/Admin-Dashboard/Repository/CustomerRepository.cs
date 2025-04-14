using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository(SanayiiContext db) : base(db) { }

        public override Customer getById<T>(T id)
        {
            return db.Customers
                .Include(c => c.IdNavigation)
                    .ThenInclude(u => u.UserPhones)
                .FirstOrDefault(c => c.Id.Equals(id.ToString()));
        }

        public override List<Customer> getAll()
        {
            return db.Customers
                .Include(c => c.IdNavigation)
                    .ThenInclude(u => u.UserPhones)
                .Where(c => c.IdNavigation.IsDeleted == false)
                .ToList();
        }


        // Custom method to get customers with their discounts
        public List<Customer> GetCustomersWithDiscounts()
        {
            return db.Customers
                .Include(c => c.IdNavigation)
                .Include(c => c.CustomerDiscounts)
                    .ThenInclude(cd => cd.Discount)
                .ToList();
        }

        // Custom method to get customers with their reviews
        public List<Customer> GetCustomersWithReviews()
        {
            return db.Customers
                .Include(c => c.IdNavigation)
                .Include(c => c.Reviews)
                .ToList();
        }
    }
}