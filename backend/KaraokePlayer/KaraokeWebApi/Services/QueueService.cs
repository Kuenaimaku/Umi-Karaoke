using KaraokeWebApi.Data;
using KaraokeWebApi.Models;
using KaraokeWebApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KaraokeWebApi.Services
{
    public class QueueService : IQueueService
    {
        private readonly DataContext _context;
        private readonly ISongsService _songsService;

        private readonly HashSet<QueueStatus> _activeStatuses = new HashSet<QueueStatus>() { QueueStatus.Queued, QueueStatus.Playing };

        public QueueService(DataContext context, ISongsService songsService)
        {
            _context = context;
            _songsService = songsService;
        }

        public async Task<IEnumerable<QueueDTO>> GetQueueAsync()
        {
            var queue = await _context.Queue.Where(x => _activeStatuses.Contains(x.Status)).OrderBy(x => x.Timestamp).ToListAsync();

            var response = new List<QueueDTO>();

            foreach (var item in queue)
            {
                var song = await _songsService.GetSongAsync(item.SongId);

                if (song != null)
                {
                    response.Add(new QueueDTO
                    {
                        Id = item.Id,
                        User = item.User,
                        SongId = item.SongId,
                        Status = item.Status,
                        Timestamp = item.Timestamp,
                        Song = song
                    });
                }
            }

            return response;
        }

        public async Task<QueueDTO> AddSongAsync(string user, string songId)
        {
            if (await _context.Queue.AnyAsync(x => x.SongId == songId && x.User == user && _activeStatuses.Contains(x.Status)))
            {
                throw new InvalidOperationException("Song already in queue!");
            }

            var song = await _songsService.GetSongAsync(songId);

            if (song == null)
            {
                throw new InvalidOperationException("Song not found!");
            }

            var queueItem = new QueueItem
            {
                Id = Guid.NewGuid(),
                SongId = songId,
                User = user,
                Status = QueueStatus.Queued,
                Timestamp = DateTime.Now.ToUniversalTime(),
            };

            await _context.Queue.AddAsync(queueItem);
            await _context.SaveChangesAsync();

            return new QueueDTO
            {
                Id = queueItem.Id,
                User = queueItem.User,
                SongId = queueItem.SongId,
                Status = queueItem.Status,
                Timestamp = queueItem.Timestamp,
                Song = song
            };
        }

        public async Task<QueueDTO> RemoveSongAsync(Guid id)
        {
            
             var queueItem = await _context.Queue.SingleOrDefaultAsync(x => x.Id == id);
            if (queueItem != null)
            {
                queueItem.Status = QueueStatus.Cancelled;
                await _context.SaveChangesAsync();
                var song = await _songsService.GetSongAsync(queueItem.SongId);
                return new QueueDTO
                {
                    Id = queueItem.Id,
                    User = queueItem.User,
                    SongId = queueItem.SongId,
                    Status = queueItem.Status,
                    Timestamp = queueItem.Timestamp,
                    Song = song
                };
            }

            throw (new InvalidOperationException("Request not found!"));

        }

        public async Task RemoveAllSongsAsync()
        {
            var queue = await _context.Queue.ToListAsync();
            _context.Queue.RemoveRange(queue);
            await _context.SaveChangesAsync();
        }

        public async Task<QueueDTO> SetNowPlayingAsync(QueueDTO queueDTO)
        {
            var result = await _context.Queue.SingleOrDefaultAsync(q => q.Id == queueDTO.Id);
            if (result != null)
            {
                var previousSongs = await _context.Queue.Where(q => _activeStatuses.Contains(q.Status) && q.Timestamp <= queueDTO.Timestamp).ToListAsync();
                foreach ( var previousSong in previousSongs )
                {
                    previousSong.Status = QueueStatus.Finished;
                }
                await _context.SaveChangesAsync();

                result.Status = QueueStatus.Playing;
                await _context.SaveChangesAsync();

                queueDTO.Status = QueueStatus.Playing;
                return queueDTO;
            }

            throw new InvalidOperationException("Could not update Queue!");
        }
    }
}
