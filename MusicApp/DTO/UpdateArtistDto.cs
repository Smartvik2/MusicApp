namespace MusicApp.DTO
{
    public class UpdateArtistDto
    {
        public int Id { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int ExperienceInYears { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;  
        public string PortfolioUrl { get; set; } = string.Empty;
    }
}
