using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Enums;
using MusicApp.Helpers;
using MusicApp.Interfaces;
using MusicApp.Models;



namespace MusicApp.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public ArtistService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        //Users can request to become artists which could only be approved by admins
        // This method creates an artist profile and notifies admins about the new request
        // If the user is already an artist, it returns a message indicating that
        // Please note that the artist profile is not approved by default and needs admin approval
        // Pagination is not applied here as this is a single request operation
        public async Task<string> RequestArtist(string userId, CreateArtistDto dto)
        {            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";
            var userRole = await _userManager.IsInRoleAsync(user, "Artist");
            if (userRole)
            return "You are already an Artist";


            if (!Enum.TryParse<AvailabilityStatus>(dto.Availability, true, out var parsedStatus))
            {
                return $"Invalid availability status: '{dto.Availability}'. Valid values: {string.Join(", ", Enum.GetNames(typeof(AvailabilityStatus)))}";
            }

            if (!Enum.TryParse<GenreType>(dto.Genre, true, out var parsedGenre))
            {
                return $"Invalid genre: {dto.Genre}";
            }


            var artist = new ArtistProfile
            {
                UserId = userId,
                Genre = parsedGenre,
                ExperienceInYears = dto.ExperienceInYears,
                Bio = dto.Bio,
                Availability = parsedStatus,
                PortfolioUrl = dto.PortfolioUrl,
                IsApproved = false, // Default to false until approved by admin
            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                await _notificationService.SendNotificationAsync(
                    admin.Id,
                    $"New artist request from {user.FullName}.",
                    "Artist Request");
            }

            return "Artist Profile created successfully, Please await Approval";
        }

        // This method supports search for other artist with optional filters and sorting.
        // Anybody can search for artists
        // Pagination is applied to the results
        public async Task<PaginatedResult<ArtistDto>> GetArtistsAsync(
                int page = 1,
                int pageSize = 10,
                string? name = null,
                int? id = null,
                string? genre = null,
                AvailabilityStatus? availability = null,
                double? minRating = null,
                string? sort = null)
               
        {
            var query = _context.Artists
                .Include(a => a.User)
                .Include(a => a.Reviews) // Include reviews for rating calculation
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(a => a.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(a => a.User.FullName.ToLower().Contains(name.ToLower()));
            }
            
            if (!string.IsNullOrWhiteSpace(genre) && Enum.TryParse<GenreType>(genre, true, out var parsedGenre))
            {
                query = query.Where(a => a.Genre == parsedGenre);
            }

            if (availability.HasValue)
            {
                query = query.Where(a => a.Availability == availability.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(a => a.Reviews.Any() && a.Reviews.Average(r => r.Rating) >= minRating.Value);
            }


            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "name_asc" => query.OrderBy(a => a.User.FullName),
                    "name_desc" => query.OrderByDescending(a => a.User.FullName),
                    _ => query.OrderBy(a => a.Id)
                };
            }
            else
            {
                query = query.OrderBy(a => a.Id);
            }

            var totalCount = await query.CountAsync();

            var artists = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    FullName = a.User.FullName,
                    Genre = a.Genre.ToString(),
                    ExperienceInYears = a.ExperienceInYears,
                    Bio = a.Bio,
                    Availability = a.Availability.ToString(),
                    AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : 0,
                    PortfolioUrl = a.PortfolioUrl
                })
                .ToListAsync();

            return new PaginatedResult<ArtistDto>
            {
                Items = artists,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }



        // Artists can update their profile information
        public async Task<string> UpdateArtistAsync(UpdateArtistDto dto)
        {
            var artist = await _context.Artists.FindAsync(dto.Id);
            if (artist == null) return "Artist not found";

            if (!Enum.TryParse<GenreType>(dto.Genre, true, out var parsedGenre))
            {
                return $"Invalid genre: {dto.Genre}";
            }

            artist.Genre = parsedGenre;
            artist.ExperienceInYears = dto.ExperienceInYears;
            artist.Bio = dto.Bio;

            if (!Enum.TryParse<AvailabilityStatus>(dto.Availability, true, out var parsedStatus))
            {
                return $"Invalid availability status: '{dto.Availability}'. Valid values are: {string.Join(", ", Enum.GetNames(typeof(AvailabilityStatus)))}";
            }

            artist.Availability = parsedStatus;
            artist.PortfolioUrl = dto.PortfolioUrl;

            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();

            return "Artist updated successfully";
        }

        // Artists can choose to delete their account
        public async Task<string> DeleteMyAccountAsync(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return "Artist not found";

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return "Artist deleted successfully";
        }
    }
}
