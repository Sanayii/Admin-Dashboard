using AdminDashboard.Models;

namespace AdminDashboard.Repository
{
    public class ArtisanRepository : GenericRepository<Artisan>
    {
        public ArtisanRepository(SanayiiContext db) : base(db) { }

        public List<Artisan> GetTopRatedArtisans()
        {
            return db.Artisans.OrderByDescending(a => a.Rating).ToList();
        }
    }
}
