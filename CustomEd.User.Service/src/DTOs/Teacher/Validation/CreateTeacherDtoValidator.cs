using CustomEd.User.Service.DTOs;
using FluentValidation;

namespace CustomEd.User.Service.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        public CreateTeacherDtoValidator()
        {
            RuleFor(dto => dto.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(50)
                .WithMessage("First name must not exceed 50 characters.");
            RuleFor(dto => dto.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(50)
                .WithMessage("Last name must not exceed 50 characters.");

            RuleFor(dto => dto.DateOfBirth)
                .NotNull()
                .WithMessage("Date of birth is required.")
                .LessThan(System.DateTime.Now)
                .WithMessage("Date of birth cannot be later than today.");

            RuleFor(dto => dto.Department)
                .NotEmpty()
                .WithMessage("Department is required.")
                .IsInEnum()
                .WithMessage("Invalid department.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 digits.");

            RuleFor(dto => dto.JoinDate)
                .NotNull()
                .WithMessage("Join date is required.")
                .LessThan(System.DateTime.Now)
                .WithMessage("Join date cannot be later than today.");
                
            RuleFor(dto => dto.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
            RuleFor(dto => dto.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");
        }
    }
}
