namespace Admin_Dashboard.ViewModels
{
    public class ContractViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxViolationsAllowed { get; set; }
        public string Status { get; set; }
        public string ArtisanFullName { get; set; }
    }
}
