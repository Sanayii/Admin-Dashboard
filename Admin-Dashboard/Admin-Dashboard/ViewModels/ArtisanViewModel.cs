using System.ComponentModel.DataAnnotations;

namespace Admin_Dashboard.ViewModels
{
    public class ArtisanViewModel
    {
        public string? Id { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public int? Age { get; set; }

        public string Email { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Government { get; set; }

        [Required]
        public string NationalityId { get; set; }

        public int Rating { get; set; }

        public int CategoryId { get; set; }

        public List<string> Phones { get; set; } = new List<string>();
    }
}
