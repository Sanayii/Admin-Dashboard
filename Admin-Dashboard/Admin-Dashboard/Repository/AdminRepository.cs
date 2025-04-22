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
        public  void AddAdmin (Admin admin)
        {
            
            db.Admins.Add(admin);
        }

    }
}