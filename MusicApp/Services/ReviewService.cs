using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;
using MusicApp.Interfaces;

namespace MusicApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Users can submit reviews for artists
        // Reviews include a rating (1-5) and an optional comment
        public async Task<string> SubmitReviewAsync(string userId, CreateReviewDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return "User not found";

            if (dto.Rating < 1 || dto.Rating > 5)
                return "Rating must be between 1 and 5";

            var artist = await _context.Artists
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.FullName == dto.ArtistName);

            if (artist == null) return "Artist not found";

            var review = new Review
            {
                ReviewerId = user.Id,
                ArtistId = artist.Id,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return "Review submitted successfully";
        }

        // This method retrieves all reviews for a specific artist
        // Accessible to anyone
        public async Task<List<ReviewDto>> GetReviewsForArtistAsync(int artistId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ArtistId == artistId)
                .Include(r => r.Reviewer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewList = reviews.Select(r => new ReviewDto
            {
                UserName = r.Reviewer.FullName ?? "",
                Comment = r.Comment,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy")
            }).ToList();

            return reviewList;
        }

    }
}
