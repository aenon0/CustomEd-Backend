using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using FluentValidation;


namespace CustomEd.User.Service.Validators;

public class VerifyPasswordForForgotPasswordDtoValidator: AbstractValidator<VerifyPasswordForForgotPasswordDto>
{        
        private readonly IGenericRepository<ForgotPasswordOtp> _forgotPasswordOtpRepository;
        public VerifyPasswordForForgotPasswordDtoValidator(IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository)
        {
            _forgotPasswordOtpRepository = forgotPasswordOtpRepository;
            RuleFor(dto => dto.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(async (email, cancellation) => 
                {
                    var forgotPasswordOtpItem = await _forgotPasswordOtpRepository.GetAsync(i => i.Email == email);
                    return forgotPasswordOtpItem != null;
                })
                .WithMessage("Email must be unique.");
            
            RuleFor(dto => dto.OtpCode)
                .NotEmpty()
                .WithMessage("Otp code is required.")
                .Length(4)
                .WithMessage("Otp code must be exactly 4 characters long.");
        }
}
