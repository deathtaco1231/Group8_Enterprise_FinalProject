namespace Group8_Enterprise_FinalProject.Messages
{
    public class TournamentRegistrationDetails
    {
        public int TournamentRegistrationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}