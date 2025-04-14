using Hangfire;
using System;
using System.Net;
using System.Net.Mail;
using Group8_Enterprise_FinalProject.Entities;

namespace Group8_Enterprise_FinalProject.Services
{
    public class TournamentManagerService : ITournamentManagerService
    {
        private const string fromAddress = "etourneypro@gmail.com";
        private const string appPassword = "pesl opgp dbcg tsdl";

        private readonly IBackgroundJobClient _backgroundJobClient;

        // Inject IBackgroundJobClient via the constructor.
        public TournamentManagerService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void SendPlayerEmail(string toAddress, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromAddress, appPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
            };
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toAddress);

            smtpClient.Send(mailMessage);
        }

        public void SendPlayerEmailWithDelay(string toAddress, string subject, string body, DateTime gameTime, double minutesDiff)
        {
            // Determine when to send the email.
            var scheduledSendTime = gameTime.AddMinutes(minutesDiff);
            var delay = scheduledSendTime - DateTime.Now;

            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            // Use the injected instance to schedule the email.
            _backgroundJobClient.Schedule(() => SendPlayerEmail(toAddress, subject, body), delay);
        }

        /// <summary>
        /// Formats email sent to players upon successful registration with details passed as parameters
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="tournamentName"></param>
        /// <param name="gameName"></param>
        /// <param name="gameDate"></param>
        /// <returns></returns>
        public string FormatRegistrationEmail(string playerName, string tournamentName, int tournamentId, string gameName)
        {
            // Subject line is handled by caller, this method only concerns the body text with HTML wrapping (basically auto-formatting for caller)
            string body = $@"
                    <h1>Hello 
            {playerName}, </h1><p>We wanted to let you know that your registration for the {tournamentName} tournament has been confirmed, and you are scheduled to play a game!</p>
                    <p>Please click <a href=""https://localhost:5001/Tournaments/
            {tournamentId}"">here</a> for more details about the tournament. You will be playing {gameName}. You will receive a notice when your scheduled games are about to begin.</p>
                    <p>Sincerely,<br>ETourneyPro</p>";

            return body;
        }

        /// <summary>
        /// Formats email sent to players with a reminder of the game they are registered to play
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="tournamentName"></param>
        /// <param name="gameId"></param>
        /// <param name="gameName"></param>
        /// <param name="inverseTimeDiff"></param>
        /// <returns></returns>
        public string FormatReminderEmail(string playerName, string tournamentName, int gameId, string gameName, double inverseTimeDiff)
        {
            // Subject line is handled by caller, this method only concerns the body text with HTML wrapping (basically auto-formatting for caller)
            string body = $@"
                    <h1>Hello 
            {playerName}, </h1><p>We wanted to remind you that you are schedueled to play {gameName} in the tournament {tournamentName}.</p>
                    <p>Please click <a href=""https://localhost:5001/Games/
            {gameId}"">here</a> for more details. Your game is scheduled to start in {(-inverseTimeDiff).ToString()} minutes.</p>
                    <p>Sincerely,<br>ETourneyPro</p>";

            return body;
        }
    }
}