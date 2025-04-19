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
        public override List<Artisan> getAll()
        {
            return db?.Artisans?
                   .Include(a => a.IdNavigation)
                   .ToList() ?? new List<Artisan>();
        }
    }
}
