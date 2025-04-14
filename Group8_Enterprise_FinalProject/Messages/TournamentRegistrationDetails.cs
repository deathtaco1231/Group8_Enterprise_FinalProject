namespace Group8_Enterprise_FinalProject.Messages
{
    /// <summary>
    /// A DTO-style object meant for returning the details regarding registering a player for a tournament through the API
    /// </summary>
    public class TournamentRegistrationDetails
    {
        public int TournamentRegistrationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}