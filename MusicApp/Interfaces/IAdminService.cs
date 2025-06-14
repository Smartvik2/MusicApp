using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(string userId);


    }
}
