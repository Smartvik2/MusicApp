namespace MusicApp.DTO
{
    public class RegisterDto
    {
        public String FullName { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty ;
        public String Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
