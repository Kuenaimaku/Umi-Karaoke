using KaraokeWebApi.Models;

namespace KaraokeWebApi.Services
{
    public interface ISongsService
    {
        Task DiscoverSongsAsync();
        Task<IEnumerable<Song>> GetSongsAsync();
        Task<Song> GetSongAsync(string id);
    }
}
