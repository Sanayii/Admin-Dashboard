using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class ServiceRepo : GenericRepository<Service>
    {
        public ServiceRepo(SanayiiContext db) : base(db)
        {
        }
        public override List<Service> getAll()
        {
            if (db == null) throw new ArgumentNullException("DbContext is not initialized");
            return db.Services
                .Include(s => s.Category)
                .ToList();
        }
        public override Service getById<T>(T id)
        {
            if (id is int intId)
            {
                return db?.Services?
                    .Include(s => s.Category)
                    .FirstOrDefault(s => s.Id == intId) ?? new Service();
            }
            throw new ArgumentException("Invalid type for id. Expected an integer.", nameof(id));
        }
    }
    
    
}
