using CustomEd.Contracts.OtpService.Events;
using CustomEd.OtpService.Repository;
using CustomEd.OtpService.Service;
using CustomEd.User.Events;
using CustomEd.User.Student.Events;
using MassTransit;
using MongoDB.Bson;

namespace CustomEd.OtpService;

public class SendOtpConsumer : IConsumer<SendOtpEvent>
{
    private readonly OtpRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmailService _emailService;
    public SendOtpConsumer(OtpRepository repository, IPublishEndpoint publishEndpoint, IEmailService emailService)
    {
        _emailService = emailService;
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        Console.WriteLine("CONSUMERRRRRR");
    }

    public async Task Consume(ConsumeContext<SendOtpEvent> context)
    {
        Console.WriteLine("been here");
        var message = context.Message;
    
        var item = await _repository.GetByEmailAddress(message.Email);
        string otpCode = new OtpGenerationService().GenerateOTP();
        if (item != null){
            Console.WriteLine("True");
            var otpItem = await _repository.GetByEmailAddress(message.Email);
            otpItem.OtpCode = otpCode;
            await _repository.Update(otpItem);
            Console.WriteLine("about to send the email");
            await _emailService.SendEmail(message.Email, otpCode);
            Console.WriteLine("done sending it");
        }
        else
        {
            Console.WriteLine("False");
            await _repository.Add(new Otp{
            Id = Guid.NewGuid(),
            EmailAddress = message.Email,
            OtpCode = otpCode
            });
            await _emailService.SendEmail(message.Email, otpCode);
        }
        
        Console.WriteLine("Sent the message");
        await _publishEndpoint.Publish(new OtpSentEvent
        {
            Id = item!.Id,
            Email = message.Email,
            OtpCode = otpCode
        });
        Console.WriteLine("Publish the OtpGeneratedEvent");

        
        
    }
}
