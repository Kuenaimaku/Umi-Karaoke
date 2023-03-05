using KaraokeLib.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KaraokeLib.Services
{
    public class SongDiscoveryService : ISongDiscoveryService
    {
        private readonly Dictionary<string,Song> _SongLibrary = new Dictionary<string,Song>();

        public SongDiscoveryService(IConfiguration configuration) 
        {
            var directoryPath = configuration["SongsDirectory"];
            var directory = Directory.GetDirectoryRoot(directoryPath);
            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files.Where(x => x.EndsWith(".cdg"))) 
            {
                var x = file.ToString().Split(" - ");
                var song = new Song
                {
                    Id = x[0],
                    Artist = x[1],
                    Title = x[2],
                    Working = true,
                };

                _SongLibrary.Add(song.Id, song);
            }
        }

        public IEnumerable<string> GetSongs()
        {
            return _SongLibrary.Keys;
        }
    }
}
