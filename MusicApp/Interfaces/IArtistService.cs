using MusicApp.DTO;
using MusicApp.Helpers;
using MusicApp.Models;

namespace MusicApp.Interfaces
{
    public interface IArtistService
    {
       
        Task<PaginatedResult<ArtistDto>> GetArtistsAsync(int page = 1, int pageSize = 10, 
            string? name = null, 
            int? id = null, string? genre = null,
            AvailabilityStatus? availability = null,
            double? minRating = null, string? sort = null);

        Task<string> RequestArtist(string userId, CreateArtistDto dto);
        Task<string> UpdateArtistAsync(UpdateArtistDto dto);
        Task<string> DeleteMyAccountAsync(int id);
       

    }
}
