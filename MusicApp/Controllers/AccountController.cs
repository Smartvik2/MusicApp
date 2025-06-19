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

        // Get user profile
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token: no user ID found.");

            var user = await _userManager.Users
                .Include(u => u.ArtistProfile) 
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? user.Role ?? "User";

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName ?? "",
                role = role
            };

            if (role == "Artist" && user.ArtistProfile != null)
            {
                profile.ArtistProfile = new ArtistDto
                {
                    Id = user.ArtistProfile.Id,
                    FullName = user.FullName ?? "",
                    Genre = user.ArtistProfile.Genre.ToString(),
                    ExperienceInYears = user.ArtistProfile.ExperienceInYears,
                    Bio = user.ArtistProfile.Bio,
                    Availability = user.ArtistProfile.Availability.ToString(),
                    PortfolioUrl = user.ArtistProfile.PortfolioUrl,
                    IsApproved = user.ArtistProfile.IsApproved
                };
            }

            return Ok(profile);
        }

    }
}
