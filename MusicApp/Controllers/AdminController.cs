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
            try
            {
                var users = await _adminService.GetAllUsersAsync(page, pageSize, search, role, sort);
                if (users == null)
                    return NotFound(new { message = "Users not found" });

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching users", error = ex.Message });
            }
        }
           



        [HttpGet("admin/pending-artists")]
        public async Task<IActionResult> GetPendingArtists([FromQuery] ArtistQueryParameters parameters)
        {
            try
            {
                var result = await _adminService.GetPendingArtistsAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching pending artists", error = ex.Message });
            }
        }

        [HttpPost("approve-artist")]
        public async Task<IActionResult> ApproveArtist(int artistId)
        {
            try
            {
                var result = await _adminService.ApproveArtistAsync(artistId);
                if (result.Contains("not found"))
                    return NotFound(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error approving artist", error = ex.Message });
            }
        }

        [HttpPost("admin/reject-artist")]
        public async Task<IActionResult> RejectArtist([FromBody] RejectArtistDto dto)
        {
            try
            {
                var result = await _adminService.RejectArtistAsync(dto.ArtistId, dto.Reason);
                if (result.Contains("not found"))
                    return NotFound(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error rejecting artist", error = ex.Message });
            }
        }

        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDto dto)
        {
            try
            {
                var result = await _adminService.MakeAdminAsync(dto.UserId);
                if (result.Contains("not found"))
                    return NotFound(new { message = result });

                if (result.Contains("already"))
                    return BadRequest(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error making user admin", error = ex.Message });
            }
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var result = await _adminService.DeleteReviewAsync(reviewId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting review", error = ex.Message });
            }
        }


        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(userId);
                if (!result)
                    return NotFound(new { message = "User not found or deletion failed" });

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
            }
        }

        
    }
}
