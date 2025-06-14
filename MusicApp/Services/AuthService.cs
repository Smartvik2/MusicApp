using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MusicApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email,
                Role = dto.Role
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if(!result.Succeeded)
            {
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }
            await _userManager.AddToRoleAsync(user, dto.Role);
            return "User registered Successfully";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user.Email == null)
                return "Invalid Email";
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return "Invalid Password";
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds,
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"]
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
