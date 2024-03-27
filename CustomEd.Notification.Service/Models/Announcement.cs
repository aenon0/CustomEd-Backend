using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service.Models;

public class Announcement : BaseEntity
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? attachements { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid  ClassRoomId { get; set; }
}
