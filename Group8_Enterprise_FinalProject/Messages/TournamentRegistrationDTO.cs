namespace Group8_Enterprise_FinalProject.Messages
{
    /// <summary>
    /// A DTO-style object meant for registering a player for a tournament through the API
    /// </summary>
    public class TournamentRegistrationDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
