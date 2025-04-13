using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class AdminRepository : GenericRepository<Admin>
    {
        public AdminRepository(SanayiiContext db) : base(db)
        {
        }
        public void edit(Admin entity)
        {
            db.Admins.Update(entity);
        }


        public override List<Admin> getAll()
        {
            return db.Admins
                     .Include(a => a.IdNavigation)
                         .ThenInclude(u => u.UserPhones)
                     .Where(a => !a.IdNavigation.IsDeleted)
                     .ToList();
        }


        public override Admin getById<T>(T id)
        {
            return db.Admins.Include(a => a.IdNavigation).FirstOrDefault(a => a.Id.Equals(id));
        }
    }
}
