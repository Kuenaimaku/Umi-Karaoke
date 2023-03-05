namespace KaraokeWebApi.Models.DTOs
{
    public class QueueDTO
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string SongId { get; set; }
        public QueueStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public Song Song { get; set; }

        public override string ToString()
        {
            return $"[{Status}] {Song.Artist} - {Song.Title}";
        }
    }
}
