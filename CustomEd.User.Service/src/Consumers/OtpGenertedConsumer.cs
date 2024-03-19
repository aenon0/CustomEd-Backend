using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Events;
using CustomEd.User.Service.Model;
using MassTransit;

namespace src;


public class OtpGenertedConsumer : IConsumer<OtpGeneratedEvent>
{
    private readonly IGenericRepository<Otp> _otpRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public OtpGenertedConsumer(IGenericRepository<Otp> otpRepository, IPublishEndpoint publishEndpoint)
    {
        _otpRepository = otpRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OtpGeneratedEvent> context)
    {
        var message = context.Message;
        var item = await _otpRepository.GetAsync(x => x.Email == message.Email);
        if (item != null)
        {
            item.OtpCode = message.OtpCode;
            item.UpdatedAt = DateTime.Now;
            await _otpRepository.UpdateAsync(item);
        }
        else
        {
            var otp = new Otp
            {
                Id = Guid.NewGuid(),
                Email = message.Email,
                OtpCode = message.OtpCode
            };
            await _otpRepository.CreateAsync(otp);
        }
    }
}

