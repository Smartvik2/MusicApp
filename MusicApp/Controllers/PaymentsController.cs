using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;



namespace MusicApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(IPaymentService paymentService, UserManager<ApplicationUser> userManager)
        {
            _paymentService = paymentService;
            _userManager = userManager;
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var clientSecret = await _paymentService.CreatePaymentIntentAsync(userId, dto);
                return Ok(new { clientSecret });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment intent.", error = ex.Message });
            }
        }

        [HttpGet("user/history")]
        public async Task<IActionResult> GetUserPaymentHistory()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var history = await _paymentService.GetUserPaymentHistoryAsync(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving payment history.", error = ex.Message });
            }
        }

        [HttpGet("artist/history-earnings")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> GetArtistEarnings()
        {
            try
            {
                var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (artistId == null) return Unauthorized();

                var earnings = await _paymentService.GetArtistEarningsAsync(artistId);
                return Ok(earnings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving artist earnings.", error = ex.Message });
            }
        }

        [HttpGet("admin/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving all payments.", error = ex.Message });
            }
        }
    }
}
