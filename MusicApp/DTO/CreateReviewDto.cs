namespace MusicApp.DTO
{
    public class CreateReviewDto
    {
        public string ArtistName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
