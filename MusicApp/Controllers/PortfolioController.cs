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

        public PortfolioController(IPortfolioService portfolioService, ApplicationDbContext context)
        {
            _portfolioService = portfolioService;
            _context = context;
        }



        [HttpPost("upload")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Upload([FromForm] PortfolioUploadDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            const long MaxSizeInBytes = 6 * 1024 * 1024; // 6MB

            if (dto.File.Length > MaxSizeInBytes)
                return BadRequest("File size exceeds the 6MB limit.");

            // Get artist profile for this user
            var artist =  await _context.Artists.FirstOrDefaultAsync(a => a.UserId == userId);
            if (artist == null)
                return BadRequest("Artist profile not found");

            var result = await _portfolioService.UploadPortfolioAsync(artist.Id, dto.File);
            return Ok(result);
        }


        [HttpGet("{artistId}")]
        [Authorize(Roles = "User, Artist, Admin")]
        public async Task<IActionResult> Get(int artistId)
        {
            var result = await _portfolioService.GetPortfolioAsync(artistId);
            return Ok(result);
        }

        [Authorize(Roles = "Artist")]
        [HttpDelete("{portfolioId}")]
        public async Task<IActionResult> Delete(int portfolioId)
        {
            var result = await _portfolioService.DeletePortfolioAsync(portfolioId);
            return Ok(result);
        }
    }
}
 