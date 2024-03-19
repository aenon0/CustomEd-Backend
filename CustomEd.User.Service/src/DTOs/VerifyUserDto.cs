namespace CusotmEd.User.Servce.DTOs;

public class VerifyUserDto
{
    public string Email { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
}
