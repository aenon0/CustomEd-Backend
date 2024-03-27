using MassTransit;

namespace CustomEd.Notification.Service;

public class ClassRoomCreatedConsumer : IConsumer<ClassroomCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public ClassRoomCreatedConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
    {
        ///update the classroom database 
        throw new NotImplementedException();
    }
} 
