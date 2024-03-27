using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service.Models;

public class ClassRoom : BaseEntity
{
    public string? Name { get; set; }
    // public string? Description { get; set; }
    // public List<string>? Students { get; set; }
    public Guid CreatorId { get; set; }
    public List<Guid> MemberIds { get; set; } = new List<Guid>();
}
