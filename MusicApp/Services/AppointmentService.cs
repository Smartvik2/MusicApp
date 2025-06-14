using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;


namespace MusicApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BookAppointmentAsync(string userId, CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                UserId = userId,
                ArtistId = dto.ArtistId,
                AppointmentDate = dto.AppointmentDate,
                DurationInMinutes = dto.DurationInMinutes,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AppointmentDto>> GetUserAppointmentsAsync(string userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ArtistId = a.ArtistId,
                    AppointmentDate = a.AppointmentDate,
                    DurationInMinutes = a.DurationInMinutes,
                    Status = a.Status
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetArtistAppointmentsAsync(string artistId)
        {
            return await _context.Appointments
                .Where(a => a.ArtistId == artistId)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    ArtistId = a.ArtistId,
                    AppointmentDate = a.AppointmentDate,
                    DurationInMinutes = a.DurationInMinutes,
                    Status = a.Status
                })
                .ToListAsync();
        }


    }
}
