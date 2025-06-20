using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Interfaces;
using MusicApp.DTO;
using System.Security.Claims;

namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("submit-review")]
        public async Task<IActionResult> SubmitReview([FromBody] CreateReviewDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized("User not authenticated");

                var result = await _reviewService.SubmitReviewAsync(userId, dto);
                if (result != "Review submitted successfully")
                    return BadRequest(new { message = result });
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while submitting the review: {ex.Message}");
            }
        }

        [HttpGet("artist-reviews/{artistId}")]
        public async Task<IActionResult> GetReviewsForArtist(int artistId)
        {
            try
            {

                var reviews = await _reviewService.GetReviewsForArtistAsync(artistId);
                if (reviews == null || !reviews.Any())
                    return NotFound(new { message = "No reviews found for this artist." });
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving reviews: {ex.Message}");
            }
        }
    }
}
