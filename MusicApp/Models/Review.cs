using System.ComponentModel.DataAnnotations;

namespace MusicApp.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public string ReviewerId { get; set; } = string.Empty; // User giving the review
        public int ArtistId { get; set; }                      // Artist being reviewed

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser? Reviewer { get; set; }
        public ArtistProfile? Artist { get; set; }
    }
}
