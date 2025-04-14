namespace Group8_Enterprise_FinalProject.Services
{
    // Interface defining methods for the Party Manager Service must adhere to
    // (function comments added in class implementing this interface)
    public interface ITournamentManagerService
    {
        public void SendPlayerEmail(string toAddress, string subject, string body);

        public void SendPlayerEmailWithDelay(string toAddress, string subject, string body, DateTime gameTime, double minutesDiff);

        public string FormatRegistrationEmail(string playerName, string tournamentName, int tournamentId, string gameName);

        public string FormatReminderEmail(string playerName, string tournamentName, int gameId, string gameName, double inverseTimeDiff);
    }
}
