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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Invalid user ID.");
            var result = await _reviewService.SubmitReviewAsync(userId, dto);
            return Ok(new { message = result });
        }

        [HttpGet("artist-reviews/{artistId}")]
        public async Task<IActionResult> GetReviewsForArtist(int artistId)
        {
            var reviews = await _reviewService.GetReviewsForArtistAsync(artistId);
            return Ok(reviews);
        }
    }
}
