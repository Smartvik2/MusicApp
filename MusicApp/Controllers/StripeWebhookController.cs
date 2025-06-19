using Microsoft.AspNetCore.Mvc;
using MusicApp.Data;
using Stripe;
using Microsoft.EntityFrameworkCore;
using MusicApp.Interfaces;


namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;

        public StripeWebhookController(ApplicationDbContext context, IConfiguration config, INotificationService notificationService)
        {
            _context = context;
            _config = config;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.StripePaymentId == intent.Id);
                if (payment != null)
                {
                    payment.Status = "Succeeded";
                    await _context.SaveChangesAsync();

                    await _notificationService.SendNotificationAsync(payment.UserId, "Your payment was successful", "Payment");
                }
            }

            return Ok();


        }
    }
}
