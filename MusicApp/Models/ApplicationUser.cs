using Microsoft.AspNetCore.Identity;

namespace MusicApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true;
        public ArtistProfile? ArtistProfile { get; set; }
        public ICollection<Notification> Notifications { get; set; }


    }
}
