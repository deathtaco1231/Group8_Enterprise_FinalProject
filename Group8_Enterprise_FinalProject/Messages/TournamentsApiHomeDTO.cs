namespace Group8_Enterprise_FinalProject.Messages
{
    /// <summary>
    /// Data Transfer Object for home route for API callers providing necessary config information, in this case the SPA app and Python client
    /// </summary>
    public class TournamentsApiHomeDTO
    {
        public Dictionary<string, Link>? Links { get; set; }
        public string? ApiVersion { get; set; }
        public string? Creator { get; set; }
    }
}
