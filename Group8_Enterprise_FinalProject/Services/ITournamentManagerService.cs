namespace PA2_JulianCumming_EnterpriseAppDev.Services
{
    // Interface defining methods for the Party Manager Service must adhere to
    public interface ITournamentManagerService
    {
        public void SendPlayerEmail(string toAddress, string subject, string body);

        public void SendPlayerEmailWithDelay(string toAddress, string subject, string body, DateTime gameTime, double minutesDiff);
    }
}
