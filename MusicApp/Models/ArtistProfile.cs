using MusicApp.Enums;
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
        public GenreType Genre { get; set; }

        [Required]
        public int ExperienceInYears { get; set; }

        public string? Bio { get; set; }
        public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Available;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public string? PortfolioUrl { get; set; }
        public bool IsApproved { get; set; } = false;
        public string? RejectionReason { get; set; }
    }
}
