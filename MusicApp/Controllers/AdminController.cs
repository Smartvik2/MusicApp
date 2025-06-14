using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            if (users == null)
                return NotFound(new {Message = "User not Found"});
            return Ok(users);
        }

        [HttpGet("Artists")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admin = await _adminService.GetAllUsersAsync();
            if (admin == null)
                return NotFound(new { Message = "Admin not found" });
            return Ok(admin);
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
