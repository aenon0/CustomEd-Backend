namespace CustomEd.User.Service.DTOs;

public class VerifyPasswordForForgotPasswordDto
{
    public string Email {set; get;}
    public string OtpCode {set; get;}
}
