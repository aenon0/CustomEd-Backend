using CustomEd.OtpService.Repository;
using CustomEd.User.Events;
using CustomEd.User.Student.Events;
using MassTransit;

namespace CustomEd.OtpService;

public class UserCreatedConsumer : IConsumer<StudentCreatedEvent>
{
    private readonly IOtpRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    public UserCreatedConsumer(IOtpRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        Console.WriteLine("CONSUMERRRRRR");
    }

    public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
    {
        var message = context.Message;
        var item = await _repository.ExistsByEmailAddress(message.Email);
        string otpCode = new OtpGenerationService().GenerateOTP();

        if (item == true){
            var otpItem = await _repository.GetByEmailAddress(message.Email);
            otpItem.OtpCode = otpCode;
            await _repository.Update(otpItem);
            return;
        }

        await _repository.Add(new Otp{
           Id = Guid.NewGuid(),
           EmailAddress = message.Email,
           OtpCode = otpCode
        });
        await _publishEndpoint.Publish(new OtpGeneratedEvent
        {
            Email = message.Email,
            OtpCode = otpCode
        });
    }
}
