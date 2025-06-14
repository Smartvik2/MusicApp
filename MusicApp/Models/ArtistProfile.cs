using System.ComponentModel.DataAnnotations;

namespace MusicApp.Models
{
    public class ArtistProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public string Genre { get; set; } = null!;

        [Required]
        public int ExperienceInYears { get; set; }

        public string? Bio { get; set; }
        public string? Availability { get; set; } 
        public string? PortfolioUrl { get; set; }
    }
}
