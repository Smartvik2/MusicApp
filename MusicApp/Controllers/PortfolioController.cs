using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Data;
using System.Security.Claims;

namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioService portfolioService, ApplicationDbContext context, ILogger<PortfolioController> logger)
        {
            _portfolioService = portfolioService;
            _context = context;
            _logger = logger;
        }



        [HttpPost("upload")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Upload([FromForm] PortfolioUploadDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.UserId == userId);
                if (artist == null)
                    return BadRequest("Artist profile not found");
                var result = await _portfolioService.UploadPortfolioAsync(artist.Id, dto.File);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading portfolio for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while uploading the portfolio." });
            }
        }


        [HttpGet("{artistId}")]
        [Authorize(Roles = "User, Artist, Admin")]
        public async Task<IActionResult> Get(int artistId)
        {
            try
            {
                var result = await _portfolioService.GetPortfolioAsync(artistId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching portfolio for artistId {ArtistId}", artistId);
                return StatusCode(500, new { message = "An error occurred while retrieving the portfolio." });
            }
        }

        [Authorize(Roles = "Artist")]
        [HttpDelete("{portfolioId}")]
        public async Task<IActionResult> Delete(int portfolioId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated");

                var result = await _portfolioService.DeletePortfolioAsync(portfolioId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio {PortfolioId}", portfolioId);
                return StatusCode(500, new { message = "An error occurred while deleting the portfolio." });
            }
        }
    }
}
 