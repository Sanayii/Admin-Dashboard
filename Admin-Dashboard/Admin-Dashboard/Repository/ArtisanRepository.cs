using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class ArtisanRepository : GenericRepository<Artisan>
    {
        public ArtisanRepository(SanayiiContext db) : base(db) { }

        public List<Artisan> GetTopRatedArtisans()
        {
            return db.Artisans.OrderByDescending(a => a.Rating).ToList();
        }
        // You can add any custom methods for Artisan here, for example, to get Artisan by Category:
        public IQueryable<Artisan> GetArtisansByCategory(int categoryId)
        {
            return db.Set<Artisan>().Where(a => a.CategoryId == categoryId);
        }
        public override List<Artisan> getAll()
        {
            return db.Artisans
                .Include(a => a.Category)
                .Include(a => a.IdNavigation)
                    .ThenInclude(u => u.UserPhones)
                .ToList();
        }

    }
}
