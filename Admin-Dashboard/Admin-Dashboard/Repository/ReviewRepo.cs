using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class ReviewRepo: GenericRepository<Review>
    {
        public ReviewRepo(SanayiiContext db) : base(db) { }
        public override List<Review> getAll()
        {
            if (db == null) throw new ArgumentNullException("DbContext is not initialized");
            return db.Reviews
                .Include(r => r.Artisan)
                .ThenInclude(a => a.IdNavigation)
                .Include(r => r.Customer)
                .ThenInclude(c => c.IdNavigation)
                .Include(r => r.Service)
                .ToList();
        }
        public override Review getById<T>(T id)
        {
            if (id is int intId)
            {
                return db?.Reviews?
                    .Include(r => r.Artisan)
                    .ThenInclude(a => a.IdNavigation)
                    .Include(r => r.Customer)
                    .ThenInclude(c => c.IdNavigation)
                    .Include(r => r.Service)
                    .FirstOrDefault(r => r.Id == intId) ?? new Review();
            }
            throw new ArgumentException("Invalid type for id. Expected an integer.", nameof(id));
        }
    }
}
