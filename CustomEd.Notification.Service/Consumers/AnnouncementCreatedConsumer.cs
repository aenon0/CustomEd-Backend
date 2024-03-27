using MassTransit;

namespace CustomEd.Notification.Service;
 
public class AnnouncementCreatedConsumer  : IConsumer<AnnouncementCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public AnnouncementCreatedConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Consume(ConsumeContext<AnnouncementCreatedEvent> context)
    {
        //update the annoucment databse
        //fetch classroom students
        //and send it for the ones available 

        throw new NotImplementedException();
    }
}
