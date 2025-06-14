using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IAppointmentService
    {
        Task<bool> BookAppointmentAsync(string userId, CreateAppointmentDto dto);
        Task<List<AppointmentDto>> GetUserAppointmentsAsync(string userId);
        Task<List<AppointmentDto>> GetArtistAppointmentsAsync(string artistId);
    }
}
