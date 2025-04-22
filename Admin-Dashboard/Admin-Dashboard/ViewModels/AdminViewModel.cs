using Admin_Dashboard.Models;   
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Dashboard.ViewModels 
{
    public class AdminViewModel
    {
        public string? Id { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public int? Age { get; set; }

        public string Email { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }
        public List<string> Phones { get; set; } = new List<string>();

    }
}
