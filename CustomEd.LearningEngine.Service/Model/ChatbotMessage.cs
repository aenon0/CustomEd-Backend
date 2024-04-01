using CustomEd.Shared.Model;

namespace CustomEd.LearningEngine.Service;

public class ChatbotMessage : BaseEntity
{
    public Guid StudentId {set;get;}
    public bool IsSentByBot {set;get;} = true;
    public string Message {set; get;}
}
