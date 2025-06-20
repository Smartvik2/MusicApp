using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.Interfaces;
using Newtonsoft.Json.Linq;
using Stripe;


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

            if (string.IsNullOrEmpty(webhookSecret))
            {
                return BadRequest("Webhook secret is not configured.");
            }

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
                
                var paymentIntentData = stripeEvent.Data.Object as JObject;
                var intent = paymentIntentData?.ToObject<PaymentIntent>();

                if (intent == null)
                    return BadRequest("Invalid payment intent data.");
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.StripePaymentId == intent.Id);
                if (payment != null)
                {
                    payment.Status = "Succeeded";
                    payment.PaymentDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    await _notificationService.SendNotificationAsync(payment.UserId, "Your payment was successful", "Payment");
                }
            }

            return Ok();


        }
    }
}
