namespace CustomEd.Contracts.OtpService.Events;

public class OtpSentEvent
{
    public Guid Id {set; get;}
    public string Email {set; get;}
    public string OtpCode {set; get;}
}
