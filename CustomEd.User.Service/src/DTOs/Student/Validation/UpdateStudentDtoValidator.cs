using FluentValidation;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.User.Service.Validators
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        private readonly IGenericRepository<Model.Student> _studentRepository;
        public UpdateStudentDtoValidator(IGenericRepository<Model.Student> studentRepository)
        {
            _studentRepository = studentRepository;
            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 digits.");
        }
    }
}
