using MusicApp.DTO;
using MusicApp.Helpers;

namespace MusicApp.Interfaces
{
    public interface IAppointmentService
    {
        Task<int> BookAppointmentAsync(string userId, CreateAppointmentDto dto);
        Task<PaginatedResult<AppointmentDto>> GetUserAppointmentsAsync(string userId, int page, int pageSize, string? status = null, string? sort = null);
        Task<PaginatedResult<AppointmentDto>> GetArtistAppointmentsAsync(string artistId, int page, int pageSize, string? status = null, string? sort = null);
        Task<List<AppointmentActionDto>> HandleAppointmentActionAsync(AppointmentActionDto dto,string artistId);
    }
}
