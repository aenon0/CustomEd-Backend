namespace CustomEd.User.Service.Model;
public class User : Shared.Model.BaseEntity
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Role Role { get; set; }
}