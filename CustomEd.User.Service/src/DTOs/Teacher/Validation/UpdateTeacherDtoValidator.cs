using FluentValidation;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.User.Service.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        public UpdateTeacherDtoValidator(IGenericRepository<Model.Teacher> teacherRepository)
        {
            _teacherRepository = teacherRepository;
            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 digits.");
        }
    }
}
