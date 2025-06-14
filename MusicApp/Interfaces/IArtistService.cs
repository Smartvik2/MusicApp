using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IArtistService
    {
        Task<List<ArtistDto>> GetAllArtistsAsync();
        Task<ArtistDto?> GetArtistByIdAsync(int id);
        Task<string> CreateArtistAsync(string userId, CreateArtistDto dto);
        Task<string> UpdateArtistAsync(UpdateArtistDto dto);
        Task<string> DeleteArtistAsync(int id);
        Task<List<ArtistDto>> SearchArtistsByNameAsync(string name);

    }
}
