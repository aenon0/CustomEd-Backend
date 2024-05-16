using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using FluentValidation;

namespace CustomEd.User.Service;

public class ChangePasswordRequestDtoValidator: AbstractValidator<ChangePasswordRequestDto>
{
    private readonly IGenericRepository<ForgotPasswordOtp> _forgotPasswordOtpRepository;
    public ChangePasswordRequestDtoValidator(IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository)
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
        
        RuleFor(dto => dto.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
