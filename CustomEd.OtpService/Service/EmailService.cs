using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CustomEd.OtpService.Service
{
    public class EmailService : IEmailService
    {
        private SmtpClient _smtpClient;
        private readonly string mail = Environment.GetEnvironmentVariable("EMAIL_ADDRESS"); 
        private readonly string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

        public EmailService()
        {
            _smtpClient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            
        }

        public async Task SendEmail(string recieverEmailAddress, string message)
        {
            await _smtpClient.SendMailAsync( new MailMessage(from: mail, to: recieverEmailAddress, "CustomEd: Verification Message", message));
        }
    }
}
