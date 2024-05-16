using CustomEd;
using CustomEd.Contracts.OtpService.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Events;
using CustomEd.User.Service.Model;
using MassTransit;

namespace src;


public class OtpGenertedConsumer : IConsumer<OtpSentEvent>
{
    private readonly IGenericRepository<Otp> _otpRepository;
    private readonly IGenericRepository<ForgotPasswordOtp> _forgotPasswordOtpRepository;
    private readonly IGenericRepository<Student> _studentRepository;
    private readonly IGenericRepository<Teacher> _teacherRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public OtpGenertedConsumer(IGenericRepository<Otp> otpRepository, IPublishEndpoint publishEndpoint, IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository, IGenericRepository<Student> studentRepository, IGenericRepository<Teacher> teacherRepository)
    {
        _otpRepository = otpRepository;
        _publishEndpoint = publishEndpoint;
        _forgotPasswordOtpRepository = forgotPasswordOtpRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task Consume(ConsumeContext<OtpSentEvent> context)
    {
        var message = context.Message;
        var student = await _studentRepository.GetAsync(x => x.Email == message.Email);
        var teacher = await _teacherRepository.GetAsync(x => x.Email == message.Email);
        //for the forgot password
        if((student != null && student.IsVerified == true) || (teacher != null && teacher.IsVerified == true))
        {
            var forgotPassworditem = await _forgotPasswordOtpRepository.GetAsync(x => x.Email == message.Email);
            if(forgotPassworditem == null)
            {      await _forgotPasswordOtpRepository.CreateAsync(new ForgotPasswordOtp{
                    Id  = message.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Email = message.Email, 
                    OtpCode = message.OtpCode,
                    Allowed = false
                });
            }
            else
            {
                forgotPassworditem.OtpCode = message.OtpCode;
                forgotPassworditem.UpdatedAt = DateTime.Now;
                await _forgotPasswordOtpRepository.UpdateAsync(forgotPassworditem);
            }
        }
        //for account verification 
        else
        {
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
                    Id = message.Id,
                    Email = message.Email,
                    OtpCode = message.OtpCode
                };
                await _otpRepository.CreateAsync(otp);
            }
        }
    }
}

