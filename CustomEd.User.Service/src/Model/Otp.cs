namespace CustomEd.User.Service.Model;

public class Otp : Shared.Model.BaseEntity
{
     public string EmailAddress { get; set; }
    public string OtpCode { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
