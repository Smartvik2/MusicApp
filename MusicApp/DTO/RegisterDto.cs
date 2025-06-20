using System.ComponentModel.DataAnnotations;

namespace MusicApp.DTO
{
    public class RegisterDto
    {
        public String FullName { get; set; } = string.Empty;
        [Required]
        public String Email { get; set; } = string.Empty ;
        [Required]
        public String Password { get; set; } = string.Empty;
        
    }
}
