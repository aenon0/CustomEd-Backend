using CustomEd.Shared.JWT.Contracts;

namespace CustomEd.User.Service.Common;

public class UserDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public IdentityRole Role { get; set; }
    public string? Token { get; set; }
    public string? ImageUrl { get; set; }
    
}
