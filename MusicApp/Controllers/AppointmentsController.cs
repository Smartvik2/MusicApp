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

        [Authorize(Roles = "User, Admin")]
        [HttpPost("book-appointment")]
        public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(new { Message = "User not found" });

            var appointmentId = await _service.BookAppointmentAsync(userId, dto);

            if (appointmentId > 0)
                return Ok(new { message = "Appointment booked", appointmentId });

            return BadRequest("Booking failed.");
        }


        [Authorize]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = "desc")
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
                return NotFound(new { Message = "User appointment not found" });
            var appointments = await _service.GetUserAppointmentsAsync(UserId, page, pageSize);
            return Ok(appointments);
        }

        [Authorize(Roles = "Artist")]
        [HttpGet("artist-appointments")]
        public async Task<IActionResult> GetArtistAppointments([FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = "desc")
        {
            var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = await _service.GetArtistAppointmentsAsync(artistId, page, pageSize);
            return Ok(appointments);
        }

        [HttpPost("Handle-appointments")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> HandleAppointment([FromBody] AppointmentActionDto dto)
        {
            var artistId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.HandleAppointmentActionAsync(dto, artistId);
            return Ok(result);
        }


    }
}
