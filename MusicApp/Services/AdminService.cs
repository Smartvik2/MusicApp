using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Helpers;
using MusicApp.Interfaces;
using MusicApp.Models;

namespace MusicApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        // All methods in this service are for Admins only

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }


        // Admin can view all users with pagination, search, filtering by role, and sorting
        // Pagination is applied with page and pageSize parameters
        // Search is applied to FullName and Email fields
        public async Task<PaginatedResult<AdminUserDto>> GetAllUsersAsync(int page, int pageSize, string? search, string? role, string? sort)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            var users = await query.ToListAsync();

            if (!string.IsNullOrEmpty(role))
            {
                var filteredUsers = new List<ApplicationUser>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(role))
                    {
                        filteredUsers.Add(user);
                    }
                }

                users = filteredUsers;
            }


            // Sorting
            users = sort?.ToLower() == "asc"
                ? users.OrderBy(u => u.FullName).ToList()
                : users.OrderByDescending(u => u.FullName).ToList();

            var totalCount = users.Count;

            var paginated = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userDtos = new List<AdminUserDto>();
            foreach (var user in paginated)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new AdminUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    Roles = roles.ToList()
                });
            }

            return new PaginatedResult<AdminUserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        //Admins can view all pending artists with pagination, search, filtering by genre, and sorting
        public async Task<PaginatedResult<PendingArtistDto>> GetPendingArtistsAsync(ArtistQueryParameters parameters)
        {
            var query = _context.Artists
                .Include(a => a.User)
                .Where(a => !a.IsApproved)
                .AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                string searchLower = parameters.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.User.FullName.ToLower().Contains(searchLower) ||
                    a.User.Email.ToLower().Contains(searchLower) ||
                    a.Genre.ToString().ToLower().Contains(searchLower));
            }

            // 🔃 Order
            switch (parameters.OrderBy?.ToLower())
            {
                case "email":
                    query = parameters.Descending ? query.OrderByDescending(a => a.User.Email) : query.OrderBy(a => a.User.Email);
                    break;
                case "genre":
                    query = parameters.Descending ? query.OrderByDescending(a => a.Genre) : query.OrderBy(a => a.Genre);
                    break;
                default:
                    query = parameters.Descending ? query.OrderByDescending(a => a.User.FullName) : query.OrderBy(a => a.User.FullName);
                    break;
            }

            // 📄 Pagination
            var totalCount = await query.CountAsync();
            var artists = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var result = new PaginatedResult<PendingArtistDto>
            {
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize,
                Items = artists.Select(a => new PendingArtistDto
                {
                    ArtistId = a.Id,
                    FullName = a.User.FullName ?? "",
                    Email = a.User.Email ?? "",
                    Genre = a.Genre.ToString(),
                    ExperienceInYears = a.ExperienceInYears,
                    Bio = a.Bio ?? "",
                    PortfolioUrl = a.PortfolioUrl ?? "",
                }).ToList()
            };

            return result;
        }

        //Admins can approve an User's request to become an artist
        // Users get notified when their request is approved
        public async Task<string> ApproveArtistAsync(int artistId)
        {
            var artist = await _context.Artists
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == artistId && !a.IsApproved);

            if (artist == null)
                return "Artist not found or already approved.";

            var isAlreadyArtist = await _userManager.IsInRoleAsync(artist.User, "Artist");
            if (isAlreadyArtist)
                return "User is already an Artist.";

            artist.IsApproved = true;
            artist.Availability = AvailabilityStatus.Available;

            await _userManager.AddToRoleAsync(artist.User, "Artist");
            await _context.SaveChangesAsync();

            //Notify the artist: request approved
            await _notificationService.SendNotificationAsync(
            artist.UserId,
            "Your request to become an artist has been approved!",
            "Approval");

            return $"Artist '{artist.User.FullName}' has been approved.";
        }

        //Admins can reject an User's request to become an artist
        // Users get notified when their request is rejected with a reason
        public async Task<string> RejectArtistAsync(int artistId, string reason)
        {
            var artist = await _context.Artists
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == artistId && !a.IsApproved);

            if (artist == null)
                return "Artist not found or already processed.";

            artist.IsApproved = false;
            artist.RejectionReason = reason;

            await _context.SaveChangesAsync();
            // Notify the artist: request rejected
            await _notificationService.SendNotificationAsync(
            artist.UserId,
            $"Your request to become an artist was rejected. Reason: {reason}",
            "Rejection");

            return $"Artist '{artist.User.FullName}' has been rejected.";
        }

        // Admins can promote a user to an admin role
        // Users get notified when they are promoted to admin
        public async Task<string> MakeAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found.";

            var isAlreadyAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAlreadyAdmin)
                return "User is already an admin.";

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return "Failed to promote user to Admin.";

            if (result.Succeeded)
            {
                
                var notification = new Notification
                {
                    UserId = user.Id,

                    Message = "Congratulations! You have been promoted to Admin.",

                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

                return result.Succeeded ? "User promoted to Admin successfully." : "Failed to promote user to Admin.";
            
        }

        // Admins can remove the admin role from a user
        public async Task<string> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
                return "User is not an Admin";

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded ? "User demoted from Admin successfully." : "Failed to remove admin role.";

        }

        // Admins can delete a review by its ID
        public async Task<string> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return "Review not found";

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return "Review deleted successfully";
        }

        // Admins can delete a user by their ID
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
