using KaraokeWebApi.Models;
using KaraokeWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace KaraokeWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly IQueueService _QueueService;
        public QueueController(ILogger<QueueController> logger, IQueueService QueueService)
        {
            _logger = logger;
            _QueueService = QueueService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var response = await _QueueService.GetQueueAsync();
            return Ok(response);
        }

        [HttpGet("Clear")]
        public async Task<IActionResult> Clear()
        {
            await _QueueService.RemoveAllSongsAsync();
            return Ok();
        }

    }
}
