using Admin_Dashboard.Models;

namespace Admin_Dashboard.Repository
{
    public class ReviewRepository:GenericRepository<Review>
    {
        public ReviewRepository(SanayiiContext db) : base(db) { }
        public List<Review> UnReviewed_Reviews()
        {
            return db.Reviews.Where(r => r.isViolate == true && r.isReviewed == false).ToList();
        }
    }
}
