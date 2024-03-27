using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service.Models;

public class StudentAnnouncement : BaseEntity
{
    public Guid StudentId {set; get;}
    public Guid AnnouncementId {set; get;}
    public bool IsRead {set; get;} = false;
}
