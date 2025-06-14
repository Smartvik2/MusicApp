using MusicApp.Interfaces;
using MusicApp.Data;
using Microsoft.AspNetCore.Identity;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;
using MusicApp.DTO;

namespace MusicApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var UserDtos = new List<AdminUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                UserDtos.Add(new AdminUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return UserDtos;
        }

       

        public async Task<List<ArtistDto>> GetAllArtistsAsync()
        {
            var artistUsers = await _userManager.GetUsersInRoleAsync("Artist");
            var artistDtos = new List<ArtistDto>();

            foreach (var user in artistUsers)
            {
                var profile = await _context.Artists.FirstOrDefaultAsync(a => a.UserId == user.Id);

                if (profile != null)
                {
                    artistDtos.Add(new ArtistDto
                    {
                        Id = profile.Id,
                        FullName = user.FullName ?? "",
                        Genre = profile.Genre,
                        ExperienceInYears = profile.ExperienceInYears,
                        Bio = profile.Bio,
                        Availability = profile.Availability,
                        PortfolioUrl = profile.PortfolioUrl
                    });
                }
            }

            return artistDtos;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }




    }
}
