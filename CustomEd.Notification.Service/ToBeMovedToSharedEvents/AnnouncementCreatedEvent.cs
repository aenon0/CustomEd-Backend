namespace CustomEd.Notification.Service;

public class AnnouncementCreatedEvent
{
    public Guid Id { get; set; }
    public Guid ClassroomId { get; set; }
    public string AnnoucementMessage { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
