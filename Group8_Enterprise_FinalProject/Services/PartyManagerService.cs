using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Net;

namespace PA2_JulianCumming_EnterpriseAppDev.Services
{
    public class TournamentManagerService : ITournamentManagerService
    {
        //private const string cookieStr = "Footer_Message"; // This is used when getting and setting the cookie (essentially the cookies identifier)
        //private const string firstVisitMsg = "Hey, welcome to the party guest manager app!";
        private const string fromAddress = "iluvcobble@gmail.com";
        private const string appPassword = "qvtk bcsq dmhk zqxn";

        /// <summary>
        /// Updates shared footer message to either welcome user for first visit, or
        /// displaying when they first visited the site using a cookie
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        //public string UpdateFooterMessage(HttpContext context)
        //{
        //    string footerMsg = "";
        //    string cookieMsg = "";
        //    var cookieValue = context.Request.Cookies[cookieStr];

        //    if (!string.IsNullOrEmpty(cookieValue)) // When cookie is not found (hasn't been created yet)
        //    {
        //        // Essentially, add old message back into cookie again 
        //        cookieMsg = JsonConvert.DeserializeObject<string>(cookieValue);
        //        footerMsg = cookieMsg;
        //    }
        //    else
        //    {
        //        // Building message with date and time of first use of app (only when cookie is not found)
        //        cookieMsg = "Welcome back! You first used this app on: " + DateTime.Now.ToString(); 
        //        footerMsg = firstVisitMsg;
        //    }

        //    var cookieOptions = new CookieOptions
        //    {
        //        Expires = DateTimeOffset.UtcNow.AddDays(90), // Setting cookie to expire after 90 days each time it is created
        //    };
        //    var newCookieValue = JsonConvert.SerializeObject(cookieMsg);

        //    context.Response.Cookies.Append(cookieStr, newCookieValue, cookieOptions); // Re-add message to cookie, or create new cookie at this statement if cookie hasn't been created yet (append will create if it doesnt exist)
           
        //    return footerMsg;
        //}

        /// <summary>
        /// Uses a SMTP client to send emails with party invite information
        /// using the parameters passed from the method caller
        /// (Assumes valid address since Invite creation performs this validation before it is used here)
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendRegisterConfirmationEmail(string toAddress, string subject, string body)
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
    }
}
