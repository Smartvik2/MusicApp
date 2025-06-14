using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using System.Security.Claims;
namespace MusicApp.Controllers
{
    [Route("api/artist")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;
        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpPost("Create-artist")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> CreateArtist([FromBody] CreateArtistDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Invalid token.");

            var result = await _artistService.CreateArtistAsync(userId, dto);
            return Ok(new { message = result });
        }

        [HttpGet("get-artists")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _artistService.GetAllArtistsAsync();
            return Ok(artists);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArtist(int id)
        {
            var artists = await _artistService.GetArtistByIdAsync(id);
            if (artists == null)
                return NotFound(new {message = "Artist not Found"});
            return Ok(artists);
        }
        [HttpPut("Update-artist-profile")]
        [Authorize(Roles = "Artists")]
        public async Task<IActionResult> UpdateArtist([FromBody] UpdateArtistDto dto)
        {
            var result = _artistService.UpdateArtistAsync(dto);
            return Ok(new { message = result });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _artistService.DeleteArtistAsync(id);
            if (artist == null)
                return NotFound(new { message = "Artist Not Found" });
            return Ok(artist);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchArtists([FromQuery] string name)
        {
            var result = await _artistService.SearchArtistsByNameAsync(name);
            return Ok(result);
        }


    }
}
