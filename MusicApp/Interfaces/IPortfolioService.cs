using MusicApp.Models;

namespace MusicApp.Interfaces
{
    public interface IPortfolioService
    {
        Task<string> UploadPortfolioAsync(int artistId, IFormFile file);
        Task<List<ArtistPortfolio>> GetPortfolioAsync(int artistId);
        Task<string> DeletePortfolioAsync(int portfolioId);
    }
}
