namespace CustomEd.User.Service.Model;

public class Otp : Shared.Model.BaseEntity
{
     public string Email { get; set; }
    public string OtpCode { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
