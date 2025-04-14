namespace Group8_Enterprise_FinalProject.Messages
{
    public class TournamentDetails
    {
        public int TournamentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public int NumPlayersPerTeam { get; set; }
        public DateTime StartDateTime { get; set; }
        public int NumGames { get; set; }
        public string? Url { get; set; }
    }
}