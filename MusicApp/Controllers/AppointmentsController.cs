using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using System.Security.Claims;

namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;
        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "User")]
        [HttpPost("book-appointment")]
        public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(new { Message = "User not found"});
            var result = await _service.BookAppointmentAsync(userId, dto);
            return result ? Ok("Appointment booked.") : BadRequest("Booking failed.");
        }

        [Authorize]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null) 
                return NotFound(new {Message = "User appointment not found"});
            var appointments = await _service.GetArtistAppointmentsAsync(UserId);
            return Ok(appointments);
        }

        [Authorize(Roles = "Artist")]
        [HttpGet("artist-appointments")]
        public async Task<IActionResult> GetArtistAppointments()
        {
            var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _service.GetArtistAppointmentsAsync(artistId);
            return Ok(appointments);
        }

    }
}
