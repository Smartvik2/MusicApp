using MusicApp.DTO;

namespace MusicApp.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(string senderId, SendMessageDto dto);
        Task<List<MessageDto>> GetMessagesAsync(string userId1, string userId2);
    }
}
