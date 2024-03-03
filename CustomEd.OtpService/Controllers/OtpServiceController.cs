using CustomEd.OtpService.Repository;
using CustomEd.OtpService.Service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

namespace CustomEd.OtpService.Controllers
{
    [ApiController]
    [Route("api/OtpService")]
    public class OtpServiceController
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;

        public OtpServiceController(IOtpRepository otpRepository, IEmailService emailService)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        [HttpPost("VerifyOtp")]
        public async Task<bool> SendOtp(string emailAddress, string otpCode)
        {
            var otp = await _otpRepository.GetByEmailAddress(emailAddress);
            if(otp == null)
            {
                return false;
            }
            return otp.OtpCode == otpCode;
        }

        [HttpPost("SendOtp")]
        public async Task<bool> SendOtp(string emailAddress)
        {
            var otpCode = new OtpGenerationService().GenerateOTP();
            await _emailService.SendEmail(emailAddress, otpCode);
            var exists = await _otpRepository.ExistsByEmailAddress(emailAddress);
            if (exists)
            {
                var existingOtp = await _otpRepository.GetByEmailAddress(emailAddress);
                existingOtp.OtpCode = otpCode;
                await _otpRepository.Update(existingOtp);
            }
            else
            {
                await _otpRepository.Add(new Otp { Id = ObjectId.GenerateNewId(), EmailAddress = emailAddress, OtpCode = otpCode });
            }
            return true;
            
        }

        
    }
}
