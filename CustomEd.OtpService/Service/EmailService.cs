using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CustomEd.OtpService.Service
{
    public class EmailService : IEmailService
    {
        private readonly string _senderEmailAddress = Environment.GetEnvironmentVariable("SENDER_EMAIL");
        private readonly string  _password = Environment.GetEnvironmentVariable("SENDER_PASSWORD");
        private SmtpClient _client;

        public EmailService()
        {
            
            _client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmailAddress, _password),
                EnableSsl = true,
                Port = 587
            };
        }

        public Task SendEmail(string recieverEmailAddress, string message)
        {
            return _client.SendMailAsync(new MailMessage(_senderEmailAddress, recieverEmailAddress, "CustomEd: OTP Code", message));
        }
    }
}


//  SmtpClient client = new SmtpClient("smtp.ethereal.email", 587);
//             client.EnableSsl = true;
//             client.UseDefaultCredentials = false;
//             client.Credentials = new NetworkCredential("abby.wehner53@ethereal.email", "djgwPz8DPss78aakBv");

//             // Create email message
//             MailMessage mailMessage = new MailMessage();
//             mailMessage.From = new MailAddress("abby.wehner53@ethereal.email");
//             mailMessage.To.Add(toEmail);
//             mailMessage.Subject = subject;
//             mailMessage.IsBodyHtml = true;
//             StringBuilder mailBody = new StringBuilder();
//             mailBody.AppendFormat("<h1>User Registered</h1>");
//             mailBody.AppendFormat("<br />");
//             mailBody.AppendFormat("<p>Thank you For Registering account</p>");
//             mailMessage.Body = mailBody.ToString();

//             // Send email
//             client.Send(mailMessage);
