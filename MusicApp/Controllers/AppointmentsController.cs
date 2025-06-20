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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return NotFound(new { message = "User not found" });

                var appointmentId = await _service.BookAppointmentAsync(userId, dto);
                if (appointmentId > 0)
                    return Ok(new { message = "Appointment booked", appointmentId });

                return BadRequest(new { message = "Booking failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error booking appointment", error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = "desc")
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return NotFound(new { message = "User not found" });

                var appointments = await _service.GetUserAppointmentsAsync(userId, page, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching appointments", error = ex.Message });
            }
        }

        [Authorize(Roles = "Artist")]
        [HttpGet("artist-appointments")]
        public async Task<IActionResult> GetArtistAppointments([FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = "desc")
        {
            try
            {
                var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (artistId == null)
                    return NotFound(new { message = "Artist not found" });

                var appointments = await _service.GetArtistAppointmentsAsync(artistId, page, pageSize);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching artist appointments", error = ex.Message });
            }
        }

        [HttpPost("Handle-appointments")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> HandleAppointment([FromBody] AppointmentActionDto dto)
        {
            try
            {
                var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (artistId == null)
                    return NotFound(new { message = "Artist not found" });

                var result = await _service.HandleAppointmentActionAsync(dto, artistId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error handling appointment", error = ex.Message });
            }
        }


    }
}
