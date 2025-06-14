using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Data;
using MusicApp.Models;
using MusicApp.DTO;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;




namespace MusicApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: no user ID found.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? user.Role ?? "User";
            var profile = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName ?? "",
                role = role
                
            };

            if (role == "Artist")
            {
                var artist = await _context.Artists.Include(a=> a.User).FirstOrDefaultAsync(a => a.UserId == userId);
                if (artist != null)
                {
                    profile.ArtistProfile = new ArtistDto
                    {
                        Id = artist.Id,
                        FullName = artist.User.FullName ?? "",
                        Genre = artist.Genre,
                        ExperienceInYears = artist.ExperienceInYears,
                        Bio = artist.Bio,
                        Availability = artist.Availability,
                        PortfolioUrl = artist.PortfolioUrl
                    };
                }
            }

            return Ok(profile);



           
        }
    }
}
