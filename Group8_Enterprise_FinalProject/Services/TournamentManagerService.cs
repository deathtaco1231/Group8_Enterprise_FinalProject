using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Net;
using Hangfire;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IO;
using Group8_Enterprise_FinalProject.Entities;

namespace Group8_Enterprise_FinalProject.Services
{
    public class TournamentManagerService : ITournamentManagerService
    {
        private const string fromAddress = "etourneypro@gmail.com";
        private const string appPassword = "pesl opgp dbcg tsdl";

        /// <summary>
        /// Uses a SMTP client to send emails with party invite information
        /// using the parameters passed from the method caller
        /// (Assumes valid address since Invite creation performs this validation before it is used here)
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
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

        /// <summary>
        /// Schedules a task to send an email to the player passed in the parameters
        /// with a reminder of the game they are registered to play. 
        /// </summary>
        /// <param name="minutesDiff">Differential in minutes to send email from game scheduled time.
        /// Use negative value to send n minutes BEFORE game time.</param>
        public void SendPlayerEmailWithDelay(string toAddress, string subject, string body, DateTime gameTime, double minutesDiff)
        {
            // Determine time to send email based on time passed in as parameter
            var scheduledSendTime = gameTime.AddMinutes(minutesDiff);
            var delay = scheduledSendTime - DateTime.Now;

            // Ensure the delay is not negative, if so send immediately (we might change this to don't send at all)
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            // Schedule the email using Hangfire
            BackgroundJob.Schedule(() => SendPlayerEmail(toAddress, subject, body), delay);
        }

        /// <summary>
        /// Formats email sent to players upon successful registration with details passed as parameters
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="tournamentName"></param>
        /// <param name="gameName"></param>
        /// <param name="gameDate"></param>
        /// <returns></returns>
        public string FormatRegistrationEmail(string playerName, string tournamentName, int tournamentId, string gameName, DateTime gameDate)
        {
            // Subject line is handled by caller, this method only concerns the body text with HTML wrapping (basically auto-formatting for caller)
            string body = $@"
                    <h1>Hello 
            {playerName}, </h1><p>We wanted to let you know that your registration for the {tournamentName} tournament has been confirmed, and you are scheduled to play a game!</p>
                    <p>Please click <a href=""https://localhost:5001/Tournaments/
            {tournamentId}"">here</a> for more details about the tournament. You will be playing {gameName} at {gameDate.ToString("d")}.</p>
                    <p>Sincerely,<br>ETourneyPro</p>";

            return body;
        }
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
