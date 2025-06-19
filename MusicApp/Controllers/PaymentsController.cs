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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientSecret = await _paymentService.CreatePaymentIntentAsync(userId, dto);
            return Ok(new { clientSecret });
        }

        [HttpGet("user/history")]
        public async Task<IActionResult> GetUserPaymentHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _paymentService.GetUserPaymentHistoryAsync(userId);
            return Ok(history);
        }

        [HttpGet("artist/history")]
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> GetArtistEarnings()
        {
            var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var earnings = await _paymentService.GetArtistEarningsAsync(artistId);
            return Ok(earnings);
        }

        [HttpGet("admin/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }
    }
}
