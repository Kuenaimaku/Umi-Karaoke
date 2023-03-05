using KaraokeWebApi.Models.DTOs;
using KaraokeWebApi.Services;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace KaraokeWebApi.Hubs
{
    [SignalRHub]
    public class QueueHub: Hub
    {
        private readonly IQueueService _queueService;
        private readonly ILogger _logger;
        public QueueHub(IQueueService queueService, ILogger<QueueHub> logger) 
        {
            _queueService = queueService;
            _logger = logger;
        }

        public async Task AddSong(string user, string songId)
        {
            try
            {
                var queueDTO = await _queueService.AddSongAsync(user, songId);
                await Clients.All.SendAsync("SongAddedToQueue", queueDTO);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex.Message, ex);
                await Clients.Caller.SendAsync("FailedToAddSong", ex.Message);
            };
        }

        public async Task RemoveSong(Guid id)
        {
            try
            {
                var queueDTO = await _queueService.RemoveSongAsync(id);
                await Clients.All.SendAsync("SongRemovedFromQueue", queueDTO);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message, ex);
                await Clients.Caller.SendAsync("FailedToRemoveSong", ex.Message);
            };

        }

        public async Task SetNowPlaying(QueueDTO queueDTO)
        {
            try
            {
                var response = await _queueService.SetNowPlayingAsync(queueDTO);
                await Clients.All.SendAsync("SongSetToNowPlaying", response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message, ex);
                await Clients.Caller.SendAsync("FailedToSetNowPlaying", ex.Message);
            };
        }
    }
}
