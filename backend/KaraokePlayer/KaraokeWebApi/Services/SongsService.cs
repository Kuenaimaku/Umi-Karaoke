using KaraokeWebApi.Data;
using KaraokeWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using System.IO;
using System.IO.Compression;
using static System.Net.WebRequestMethods;

namespace KaraokeWebApi.Services
{
    public class SongsService : ISongsService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public SongsService(IConfiguration configuration, DataContext context) 
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task DiscoverSongsAsync()
        {
            var directoryPath = _configuration["SongsDirectory"];
            var directory = Directory.GetDirectoryRoot(directoryPath);
            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files.Where(x => x.EndsWith(".cdg")))
            {

                var x = Path.GetFileNameWithoutExtension(file).ToString().Split(" - ");
                var song = new Song
                {
                    Id = x[0],
                    Artist = x[1],
                    Title = x[2],
                    CdgFile = file,
                    Mp3File = file.Replace(".cdg", ".mp3")
                };

                if (!_context.Songs.Any(x => x.Id == song.Id))
                {
                    await _context.AddAsync(song);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Song>> GetSongsAsync()
        {
            await DiscoverSongsAsync();
            return await _context.Songs.OrderBy(x => x.Artist).ThenBy(y => y.Title).ToListAsync();
        }

        public async Task<Song> GetSongAsync(string id)
        {
            return await _context.Songs.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
