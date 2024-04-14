namespace CustomEd.LearningEngine.Service;

public class CreateChatbotMessage
{
    public Guid StudentId {set;get;}
    public bool IsSentByBot {set;get;} = true;
    public string Message {set; get;}
}
