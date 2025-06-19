using Microsoft.EntityFrameworkCore;
using MusicApp.Data;
using MusicApp.DTO;
using MusicApp.Enums;
using MusicApp.Helpers;
using MusicApp.Interfaces;
using MusicApp.Models;


namespace MusicApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public AppointmentService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        //Users can book appointments with artists
        //Artists get notified about new appointments
        public async Task<int> BookAppointmentAsync(string userId, CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                UserId = userId,
                ArtistId = dto.ArtistId,
                AppointmentDate = dto.AppointmentDate,
                DurationInMinutes = dto.DurationInMinutes,
                Status = AppointmentStatus.Pending,
                UserNote = dto.UserNote
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            await _notificationService.SendNotificationAsync(
                dto.ArtistId,
                $"You have a new appointment from a user.",
                "Booking");
            return appointment.Id;
        }

        // Users can view their appointments with artists, with optional filtering by status and sorting by date
        //Pagination is also supported
        public async Task<PaginatedResult<AppointmentDto>> GetUserAppointmentsAsync(
            string userId, int page, int pageSize, string? status = null, string? sort = null)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Artist)
                .Where(a => a.UserId == userId);

            // Optional filtering by status (case-insensitive)
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(a => a.Status == parsedStatus);
                }
                else
                {
                    throw new ArgumentException($"Invalid appointment status: '{status}'. Valid values are: {string.Join(", ", Enum.GetNames(typeof(AppointmentStatus)))}.");
                }
            }

            // Sorting by date (default is descending)
            query = sort?.ToLower() == "asc"
                ? query.OrderBy(a => a.AppointmentDate)
                : query.OrderByDescending(a => a.AppointmentDate);

            var totalCount = await query.CountAsync();

            var appointments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.Id,
                    UserId = a.UserId,
                    ArtistId = a.ArtistId,
                    AppointmentDate = a.AppointmentDate,
                    DurationInMinutes = a.DurationInMinutes,
                    Status = a.Status.ToString(),
                    UserNote = a.UserNote,
                    UserName = a.User.FullName,   
                    ArtistName = a.Artist.FullName,
                    ArtistNote = a.ArtistNote
                    
                })
                .ToListAsync();

            return new PaginatedResult<AppointmentDto>
            {
                Items = appointments,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        // Artists can view their appointments, with optional filtering by status and sorting by date
        //Pagination is also supported
        public async Task<PaginatedResult<AppointmentDto>> GetArtistAppointmentsAsync(string artistId, int page, int pageSize, string? status, string? sort)
        {
            var query = _context.Appointments
                .Where(a => a.ArtistId == artistId);

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<AppointmentStatus>(status, true, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }
            }

            // Sorting
            query = sort?.ToLower() == "asc"
                ? query.OrderBy(a => a.AppointmentDate)
                : query.OrderByDescending(a => a.AppointmentDate);

            var totalCount = await query.CountAsync();

            var appointments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.Id,
                    UserId = a.UserId,
                    ArtistId = a.ArtistId,
                    AppointmentDate = a.AppointmentDate,
                    DurationInMinutes = a.DurationInMinutes,
                    Status = a.Status.ToString()                 
                })
                .ToListAsync();

            return new PaginatedResult<AppointmentDto>
            {
                Items = appointments,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        // Artists can take actions on appointments (approve, reject, reschedule, mark as busy)
        //Users get Notified about the action taken on their appointment
        public async Task<List<AppointmentActionDto>> HandleAppointmentActionAsync(AppointmentActionDto dto, string artistId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId && a.ArtistId == artistId);

            if (appointment == null)
                throw new Exception("Appointment not found or you do not have permission to modify it.");

            if (appointment.Status != AppointmentStatus.Pending)
                throw new Exception("You can only act on pending appointments.");

            if (!string.IsNullOrEmpty(dto.ArtistNote))
            {
                appointment.ArtistNote = dto.ArtistNote;
            }

            string message = string.Empty;

            switch (dto.Action)
            {
                case AppointmentAction.Approve:
                    appointment.Status = AppointmentStatus.Approved;
                    message = "Your appointment has been approved.";
                    break;

                case AppointmentAction.Reject:
                    appointment.Status = AppointmentStatus.Rejected;
                    message = "Your appointment has been rejected.";
                    break;

                case AppointmentAction.Reschedule:
                    if (dto.NewTime == null)
                        throw new Exception("New time is required for rescheduling.");
                    appointment.AppointmentDate = dto.NewTime.Value;
                    appointment.Status = AppointmentStatus.Rescheduled;
                    message = "Your appointment has been rescheduled.";
                    break;

                case AppointmentAction.Busy:
                    appointment.Status = AppointmentStatus.Rejected;
                    message = "The artist is currently busy and cannot accept new appointments.";
                    break;

                default:
                    throw new Exception("Invalid appointment action.");
                   
            }

            await _context.SaveChangesAsync();
            await _notificationService.SendNotificationAsync(
        appointment.UserId,
        message,
        "Appointment");

            return new List<AppointmentActionDto>
    {
        new AppointmentActionDto
        {
            AppointmentId = appointment.Id,
            Action = dto.Action,
            NewTime = appointment.AppointmentDate
        }
    };
        }



    }
}
