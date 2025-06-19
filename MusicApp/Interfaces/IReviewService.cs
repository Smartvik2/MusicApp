using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IReviewService
    {
        Task<string> SubmitReviewAsync(string userId, CreateReviewDto dto);
        Task<List<ReviewDto>> GetReviewsForArtistAsync(int artistId);
    }
}
