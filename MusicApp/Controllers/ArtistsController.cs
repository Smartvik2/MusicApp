using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;
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
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateArtist([FromBody] CreateArtistDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Invalid.");

            var result = await _artistService.RequestArtist(userId, dto);
            return Ok(new { message = result });
        }       


        [HttpPut("Update-artist-profile")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> UpdateArtist([FromBody] UpdateArtistDto dto)
        {
            var result = _artistService.UpdateArtistAsync(dto);
            return Ok(new { message = result });
        }      

        [HttpGet("search-artists")]
        [Authorize]
        public async Task<IActionResult> SearchArtists([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name= null,
            [FromQuery] int? id = null,
            [FromQuery] string? genre = null,
            [FromQuery] AvailabilityStatus? availability = null,
            [FromQuery] double? minRating = null,
            [FromQuery] string? sort = null)
        {
            var artists = await _artistService.GetArtistsAsync(page, pageSize, name, id, genre, availability,minRating, sort);
            return Ok(artists);
        }



    }
}
