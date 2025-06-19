
namespace MusicApp.DTO
{
    public class ArtistDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int ExperienceInYears { get; set; }
        public string? Bio { get; set; }
        public string Availability { get; set; } = null!;
        public string? PortfolioUrl { get; set; }
        public double AverageRating { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
