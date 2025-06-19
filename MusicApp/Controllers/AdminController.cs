using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Helpers;
using MusicApp.Interfaces;


namespace MusicApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            [FromQuery] string? sort = "asc")
        {
            var users = await _adminService.GetAllUsersAsync(page, pageSize, search, role, sort);
            if (users == null)
                return NotFound(new {Message = "User not Found"});
            return Ok(users);
        }

        [HttpGet("admin/pending-artists")]
        public async Task<IActionResult> GetPendingArtists([FromQuery] ArtistQueryParameters parameters)
        {
            var result = await _adminService.GetPendingArtistsAsync(parameters);
            return Ok(result);
        }
        [HttpPost("approve-artist")]
        public async Task<IActionResult> ApproveArtist(int artistId)
        {
            var result = await _adminService.ApproveArtistAsync(artistId);
            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPost("admin/reject-artist")]
        public async Task<IActionResult> RejectArtist([FromBody] RejectArtistDto dto)
        {
            var result = await _adminService.RejectArtistAsync(dto.ArtistId, dto.Reason);
            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDto dto)
        {
            var result = await _adminService.MakeAdminAsync(dto.UserId);
            if (result.Contains("not found"))
                return NotFound(new { message = result });
            if (result.Contains("already"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var result = await _adminService.DeleteReviewAsync(reviewId);
            return Ok(new { message = result });
        }


        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result)
                return NotFound(new { message = "User not found or deletion failed" });

            return Ok(new { message = "User deleted successfully" });
        }

    }
}
