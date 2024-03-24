using CustomEd.Announcement.Service.Model;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime DateTime { get; set; }
    public List<string>? Attachments { get; set; }
    public ClassRoom? ClassRoom {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}