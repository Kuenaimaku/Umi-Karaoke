using KaraokeWebApi.Models;
using KaraokeWebApi.Models.DTOs;

namespace KaraokeWebApi.Services
{
    public interface IQueueService
    {
        Task<IEnumerable<QueueDTO>> GetQueueAsync();
        Task<QueueDTO> AddSongAsync(string user, string songid);
        Task<QueueDTO> RemoveSongAsync(Guid id);
        Task<QueueDTO> SetNowPlayingAsync(QueueDTO queueDTO);
        Task RemoveAllSongsAsync();
    }
}
