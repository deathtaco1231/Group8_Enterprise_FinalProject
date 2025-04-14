namespace Group8_Enterprise_FinalProject.Messages
{
    /// <summary>
    /// A DTO-style object meant to contain information about a tournament for use in an API
    /// </summary>
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