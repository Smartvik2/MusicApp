using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(string userId, PaymentDto dto);
        Task<IEnumerable<PaymentHistoryDto>> GetUserPaymentHistoryAsync(string userId);
        Task<IEnumerable<PaymentHistoryDto>> GetArtistEarningsAsync(string artistId);
        Task<IEnumerable<PaymentHistoryDto>> GetAllPaymentsAsync();
    }
}
