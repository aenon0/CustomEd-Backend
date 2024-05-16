namespace CustomEd.User.Service.Model;

public class ForgotPasswordOtp: Shared.Model.BaseEntity
{
    public string? Email { get; set; }
    public string? OtpCode { get; set; }
    public bool Allowed {set; get;} = false;
}
