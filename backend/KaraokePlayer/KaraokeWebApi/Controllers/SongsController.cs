using KaraokeWebApi.Models;
using KaraokeWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace KaraokeWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ILogger<SongsController> _logger;
        private readonly ISongsService _songsService;
        public SongsController(ILogger<SongsController> logger, ISongsService songsService)
        {
            _logger = logger;
            _songsService = songsService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var response = await _songsService.GetSongsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var song = await _songsService.GetSongAsync(id);
            if(song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }

        [HttpGet("{id}/mp3")]
        public async Task<IActionResult> DownloadMP3(string id)
        {
            var song = await _songsService.GetSongAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            byte[] result;

            try
            {
                var byteArray = System.IO.File.ReadAllBytes(song.Mp3File);
                return File(byteArray, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/cdg")]
        public async Task<IActionResult> DownloadCDG(string id)
        {
            var song = await _songsService.GetSongAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            byte[] result;

            try
            {
                var byteArray = System.IO.File.ReadAllBytes(song.CdgFile);
                return File(byteArray, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/zip")]
        public async Task<IActionResult> DownloadZip(string id)
        {
            var song = await _songsService.GetSongAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            try
            {
                var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(song.Mp3File, "song.mp3");
                    archive.CreateEntryFromFile(song.CdgFile, "song.cdg");
                }
                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "application/octet-stream", "karaokeSong.zip");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
