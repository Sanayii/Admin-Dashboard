namespace Admin_Dashboard.Models
{
    public class DashboardViewModel
    {
        public string ErrorMessage { get; set; }

        // Basic statistics
        public int TotalAdmins { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalArtisans { get; set; }
        public int TotalServiceCategories { get; set; }
        public int TotalServicesRequests { get; set; }

        // Orders report
        public List<OrderReport> RecentOrders { get; set; }

        // Chart data
        public ChartData ChartData { get; set; }
    }

    public class OrderReport
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
    }

    public class ChartData
    {
        public string[] Labels { get; set; }
        public int[] Values { get; set; }
    }
}