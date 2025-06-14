using MusicApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using MusicApp.DTO;
using MusicApp.Models;
using MusicApp.Data;
using Microsoft.EntityFrameworkCore;



namespace MusicApp.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ArtistService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<string> CreateArtistAsync(string userId, CreateArtistDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";

            var artist = new ArtistProfile
            {
                UserId = userId,
                Genre = dto.Genre,
                ExperienceInYears = dto.ExperienceInYears,
                Bio = dto.Bio,
                Availability = dto.Availability,
                PortfolioUrl = dto.PortfolioUrl
            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            return "Artist Profile created successfully";
        }

        public async Task<List<ArtistDto>> GetAllArtistsAsync()
        {
            var artists = await _context.Artists
                .Include(a => a.User)
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    FullName = a.User.FullName,
                    Genre = a.Genre,
                    ExperienceInYears = a.ExperienceInYears,
                    Bio = a.Bio,
                    Availability = a.Availability,
                    PortfolioUrl = a.PortfolioUrl
                })
                .ToListAsync();
            return artists;
               
        }

        public async Task<List<ArtistDto>> SearchArtistsByNameAsync(string name)
        {
            return await _context.Artists
                .Include(a => a.User)
                .Where(a => a.User.FullName.Contains(name))
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    FullName = a.User.FullName,
                    Genre = a.Genre,
                    ExperienceInYears = a.ExperienceInYears,
                    Bio = a.Bio,
                    Availability = a.Availability,
                    PortfolioUrl = a.PortfolioUrl
                })
                .ToListAsync();
        }


        public async Task<ArtistDto?> GetArtistByIdAsync(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null) return null;

            return new ArtistDto
            {
                Id = artist.Id,
                FullName = artist.User.FullName,
                Genre = artist.Genre,
                ExperienceInYears = artist.ExperienceInYears,
                Bio = artist.Bio,
                Availability = artist.Availability,
                PortfolioUrl = artist.PortfolioUrl
            };
        }

        public async Task<string> UpdateArtistAsync(UpdateArtistDto dto)
        {
            var artist = await _context.Artists.FindAsync(dto.Id);
            if (artist == null) return "Artist not found";

            artist.Genre = dto.Genre;
            artist.ExperienceInYears = dto.ExperienceInYears;
            artist.Bio = dto.Bio;
            artist.Availability = dto.Availability;
            artist.PortfolioUrl = dto.PortfolioUrl;

            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();

            return "Artist updated successfully";
        }

        public async Task<string> DeleteArtistAsync(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return "Artist not found";

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return "Artist deleted successfully";
        }
    }
}
