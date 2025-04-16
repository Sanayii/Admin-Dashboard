using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard.Models
{
    public class AppUser : IdentityUser
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public int? Age { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }

        public Boolean IsDeleted { get; set; } = false;//default value is false

        //navigation property as user can have many phone numbers.
        public virtual ICollection<UserPhone> UserPhones { get; set; } = new HashSet<UserPhone>();
    }
}
