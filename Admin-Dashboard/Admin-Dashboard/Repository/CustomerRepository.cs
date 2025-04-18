using Admin_Dashboard.Models;
namespace Admin_Dashboard.Repository
{
    public class CustomerRepository:GenericRepository<Customer>
    {
        public CustomerRepository(SanayiiContext db) : base(db)
        {
        }

        public List<Customer> GetAllCustomers()
        {
            return db.Customers.Where(c => c.IsDeleted == false).ToList();
        }
    }
    
}
