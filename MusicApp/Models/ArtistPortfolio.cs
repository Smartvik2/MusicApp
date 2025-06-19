namespace MusicApp.Models
{
    public class ArtistPortfolio
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public ArtistProfile Artist { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

