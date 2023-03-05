namespace KaraokeWebApi.Models
{
    public class QueueItem
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string SongId { get; set; }
        public QueueStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum QueueStatus
    {
        Queued,
        Playing,
        Finished,
        Cancelled
    }
}
