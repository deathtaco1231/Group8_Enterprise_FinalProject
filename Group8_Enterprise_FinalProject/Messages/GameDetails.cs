namespace Group8_Enterprise_FinalProject.Messages
{
    public class GameDetails
    {
        public int GameId { get; set; }
        public DateTime GameDateTime { get; set; }
        public string? Result { get; set; }
        public List<string> TeamNames { get; set; } = new List<string>();
        public string? Url { get; set; }
    }
}