using MusicApp.DTO;
using MusicApp.Helpers;

namespace MusicApp.Interfaces
{
    public interface IAdminService
    {
        
        Task<PaginatedResult<AdminUserDto>> GetAllUsersAsync(int page, int pageSize, string? search, string? role, string? sort);
        Task<PaginatedResult<PendingArtistDto>> GetPendingArtistsAsync(ArtistQueryParameters parameters);

        Task<string> ApproveArtistAsync(int artistId);
        Task<string> RejectArtistAsync(int artistId, string reason);
        Task<string> MakeAdminAsync(string userId);
        Task<string> RemoveAdmin(string userId);
        Task<string> DeleteReviewAsync(int reviewId);
        Task<bool> DeleteUserAsync(string userId);


    }
}
