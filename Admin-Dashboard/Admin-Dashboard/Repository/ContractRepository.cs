using Admin_Dashboard.Models;
using Microsoft.EntityFrameworkCore;


namespace Admin_Dashboard.Repository
{
    public class ContractRepository : GenericRepository<Contract>
    {
        public ContractRepository(SanayiiContext db) : base(db) { }
        //public override List<Contract> getAll()
        //{
        //    return db?.Contracts?
        //   .Include(c => c.Artisan)
        //   .ThenInclude(a => a.IdNavigation)
        //   .Where(c => c.Artisan != null && c.Artisan.IdNavigation != null)
        //   .ToList() ?? new List<Contract>();
        //}
        public override List<Contract> getAll()
        {
            if (db == null) throw new ArgumentNullException("DbContext is not initialized");

            return db.Contracts
                   .Include(c => c.Artisan)
                       .ThenInclude(a => a.IdNavigation)
                   .ToList();
        }
        
        public override Contract getById<T>(T id)
        {
            if (id is int intId)
            {
                return db?.Contracts?
                       .Include(c => c.Artisan)
                       .ThenInclude(a => a.IdNavigation)
                       .FirstOrDefault(c => c.Id == intId) ?? new Contract();
            }
            throw new ArgumentException("Invalid type for id. Expected an integer.", nameof(id));
        }


    }
}
