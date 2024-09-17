using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace QRQing.LineManager.Authentication
{
    public class EmailSender : IEmailSender
    {
        
        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException(); // Must fill in sender email and SMTP information to run
                                                 // Also must add your email to Advisor Accounts in the database to run
            var senderEmail = "";
            var client = new SmtpClient("", 587)
            {
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("", "")
            };


            return client.SendMailAsync(
                new MailMessage(from: senderEmail,
                                to: email,
                                subject,
                                message));
        }
        
    }
}
