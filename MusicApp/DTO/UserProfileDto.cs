namespace MusicApp.DTO
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string role { get; set; } = "User";
        public ArtistDto? ArtistProfile { get; set; }
    }
}
