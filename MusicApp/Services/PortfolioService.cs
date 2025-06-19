using MusicApp.Data;
using MusicApp.Interfaces;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicApp.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PortfolioService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Uploads a portfolio file for an artist
        // Only accessible to artists
        public async Task<string> UploadPortfolioAsync(int artistId, IFormFile file)
        {
            var artist = await _context.Artists.FindAsync(artistId);
            if (artist == null) return "Artist not found";

            if (file == null || file.Length == 0) return "No file uploaded";

            var uploadsFolder = Path.Combine(_env.WebRootPath, "portfolio");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var portfolio = new ArtistPortfolio
            {
                ArtistId = artistId,
                FileName = file.FileName,
                FileUrl = $"/portfolio/{fileName}",
                FileType = file.ContentType
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            return "File uploaded successfully";
        }

        // Retrieves the portfolio for a specific artist, ordered by upload date
        public async Task<List<ArtistPortfolio>> GetPortfolioAsync(int artistId)
        {
            return await _context.Portfolios
                .Where(p => p.ArtistId == artistId)
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();
        }

        // Deletes a portfolio file for an artist
        // Only accessible to artists who own the portfolio
        public async Task<string> DeletePortfolioAsync(int portfolioId)
        {
            var portfolio = await _context.Portfolios.FindAsync(portfolioId);
            if (portfolio == null) return "Portfolio not found";

            var filePath = Path.Combine(_env.WebRootPath, "portfolio", Path.GetFileName(portfolio.FileUrl));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();

            return "Portfolio deleted";
        }

    }
}
