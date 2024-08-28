using System.Net;
using System.Net.Mail;

namespace HangFireSample.Service
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.example.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUserName = "Example";
        private readonly string _smtpUser = "your-email@example.com";
        private readonly string _smtpPass = "your-email-password";

        public void SendEmail(string to, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Timeout = 20000;
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

                    var mail = new MailMessage
                    {
                        From = new MailAddress(_smtpUser, _smtpUserName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false 
                    };
                    mail.To.Add(to);

                    smtpClient.Send(mail);
                    mail.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
                throw;
            }
        }
    }
}
