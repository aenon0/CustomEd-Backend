namespace CustomEd.User.Events;

public class OtpGeneratedEvent
{
    public string Email { get; set; }
    public string OtpCode { get; set; }
}
