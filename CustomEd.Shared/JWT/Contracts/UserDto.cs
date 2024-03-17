namespace CustomEd.Shared.JWT.Contracts;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public IdentityRole Role { get; set; }
    public string? Token { get; set; }
}
