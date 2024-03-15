namespace CustomEd.OtpService.Service;

public interface IEmailService
{
    Task SendEmail(string recieverEmailAddress, string message);
}
