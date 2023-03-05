using System.ComponentModel.DataAnnotations;

namespace KaraokeWebApi.Models
{
    public class Song
    {
        [Key]
        public string Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string CdgFile { get; set; }
        public string Mp3File { get; set; }
        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }
    }
}
