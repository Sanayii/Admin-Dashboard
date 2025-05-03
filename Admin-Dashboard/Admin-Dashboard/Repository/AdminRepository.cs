using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Repository
{
    public class AdminRepository : GenericRepository<Admin>
    {
        public AdminRepository(SanayiiContext db) : base(db)
        {
        }
        public List<Admin> GetAllAdmins()
        { 
           return db.Admins.Where(Admin=>Admin.IsDeleted == false).ToList();
        }
        public IQueryable<Admin> GetAllAdminsQuery(bool includeDeleted = false)
        {
            var query = db.Admins.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(admin => admin.IsDeleted == false);
            }
            return query;
        }
        public  void AddAdmin (Admin admin)
        {
            
            db.Admins.Add(admin);
        }

    }
}