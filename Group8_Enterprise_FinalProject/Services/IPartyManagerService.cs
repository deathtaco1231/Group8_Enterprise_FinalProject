namespace PA2_JulianCumming_EnterpriseAppDev.Services
{
    // Interface defining methods for the Party Manager Service must adhere to
    public interface ITournamentManagerService
    {
        // public string UpdateFooterMessage(HttpContext context);

        public void SendRegisterConfirmationEmail(string toAddress, string subject, string body);
    }
}
