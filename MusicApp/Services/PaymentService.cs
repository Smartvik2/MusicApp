using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using Stripe;
using MusicApp.Interfaces;
using MusicApp.Models;

namespace MusicApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Users can create payment intents for their appointments
        // This method creates a payment intent using Stripe API and saves the payment details in the database
        public async Task<string> CreatePaymentIntentAsync(string userId, PaymentDto dto)
        {
           var appointment = await _context.Appointments
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            var options = new PaymentIntentCreateOptions
            {
                Amount = dto.Amount,
                Currency = dto.Currency.ToLowerInvariant(),
                Description = dto.Description ?? "Payment for appointment",
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            var payment = new Payment
            {
                StripePaymentId = intent.Id,
                UserId = userId,
                AppointmentId = dto.AppointmentId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = intent.Status,
                PaymentDate = DateTime.UtcNow,
                ArtistId = appointment.ArtistId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return intent.ClientSecret;

        }
        
        // This method fetches all payments made by the user and returns them as a list 
        public async Task<IEnumerable<PaymentHistoryDto>> GetUserPaymentHistoryAsync(string userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .Select(p => new PaymentHistoryDto
                {
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Status = p.Status,
                    CreatedAt = p.PaymentDate,
                    ArtistName = p.Artist.FullName,
                    Description = $"Payment for appointment #{p.AppointmentId}"
                })
                .ToListAsync();
        }

        // This method fetches all successful payments made to an artist and returns them as a list
        public async Task<IEnumerable<PaymentHistoryDto>> GetArtistEarningsAsync(string artistId)
        {
            return await _context.Payments
                .Where(p => p.ArtistId == artistId && p.Status == "Succeeded")
                .Select(p => new PaymentHistoryDto
                {
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Status = p.Status,
                    CreatedAt = p.PaymentDate,
                    ArtistName = p.Artist.FullName,
                    Description = $"Payment from user #{p.UserId}"
                })
                .ToListAsync();
        }

        // This method fetches all payments made in the system and returns them as a list
        public async Task<IEnumerable<PaymentHistoryDto>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Select(p => new PaymentHistoryDto
                {
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Status = p.Status,
                    CreatedAt = p.PaymentDate,
                    ArtistName = p.Artist.FullName,
                    Description = $"User #{p.UserId} -> Artist #{p.ArtistId}"
                })
                .ToListAsync();
        }


    }
}
