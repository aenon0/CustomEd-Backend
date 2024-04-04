using CustomEd.Shared.Model;
namespace CustomEd.Discussion.Service.Model;

public class Message : BaseEntity
{
    public Guid ClassroomId {set; get;}
    public Guid  SenderId {set; get;}
    public string Content {set; get;}
}




