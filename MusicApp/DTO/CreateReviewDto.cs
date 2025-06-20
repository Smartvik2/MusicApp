namespace MusicApp.DTO
{
    public class CreateReviewDto
    {
        
        public int ArtistId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
