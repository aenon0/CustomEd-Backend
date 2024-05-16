namespace CustomEd.Contracts.OtpService.Events;

public class SendOtpEvent
{
    public string Email {set; get;}
    public SendOtpEvent(string Email)
    {
        this.Email = Email;
    }
}
