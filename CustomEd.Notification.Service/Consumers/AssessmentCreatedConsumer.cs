using MassTransit;

namespace CustomEd.Notification.Service;

public class AssessmentCreatedConsumer : IConsumer<AssessmentCreatedConsumer>
{
    public Task Consume(ConsumeContext<AssessmentCreatedConsumer> context)
    {
        //update the assessment databse
        //fetch classroom students
        //and send it for the ones available 
        throw new NotImplementedException();
    }
}
