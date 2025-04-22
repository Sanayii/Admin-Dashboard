using Admin_Dashboard.Models;

namespace Admin_Dashboard.Repository
{
    public class ArtisanRepository : GenericRepository<Artisan>
    {
        public ArtisanRepository(SanayiiContext db) : base(db) { }

        public List<Artisan> GetTopRatedArtisans()
        {
            return db.Artisans.OrderByDescending(a => a.Rating).ToList();
        }
        public List <Artisan> GetAllArtisan()
        {
            return db.Artisans.Where(Artisan => Artisan.IsDeleted == false).ToList();
        }
    }
}
