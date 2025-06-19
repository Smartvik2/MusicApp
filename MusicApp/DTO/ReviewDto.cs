namespace MusicApp.DTO
{
    public class ReviewDto
    {
        public string UserName { get; set; } = null!;
        public string ArtistName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string CreatedAt { get; set; } = null!;
    }
}
