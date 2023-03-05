namespace KaraokeLib.Models
{
    public class Song
    {
        public string Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public bool Working { get; set; }

        public override string ToString()
        {
            return $"[{(Working ? "O" : "X")}] {Artist} - {Title}";
        }
    }
}
