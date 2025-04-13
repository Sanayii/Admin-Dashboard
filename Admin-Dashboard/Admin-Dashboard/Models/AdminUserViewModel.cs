using Admin_Dashboard.Models;

namespace Admin_Dashboard.Models // لو موجود في نفس فولدر Models
{
    public class AdminUserViewModel
    {
        public Admin Admin { get; set; } = new Admin();
        public User User { get; set; } = new User();
    }
}
