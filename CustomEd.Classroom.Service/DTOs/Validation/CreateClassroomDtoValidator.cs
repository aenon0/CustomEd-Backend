using FluentValidation;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;

namespace CustomEd.Classroom.Service.DTOs.Validation;
public class CreateClassroomDtoValidator : AbstractValidator<CreateClassroomDto>
{
    private readonly IGenericRepository<Teacher> _teacherRepository;

    public CreateClassroomDtoValidator(IGenericRepository<Teacher> teacherRepository)
    {
        _teacherRepository = teacherRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(1, 20).WithMessage("Name must be between 1 and 20 characters.");

        RuleFor(x => x.CourseNo)
            .NotEmpty().WithMessage("CourseNo is required.")
            .Length(1, 50).WithMessage("CourseNo must be between 1 and 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(1, 500).WithMessage("Description must be between 1 and 500 characters.");

        RuleFor(x => x.CreatorId)
            .NotEmpty().WithMessage("CreatorId is required.")
            .MustAsync(async (creatorId, cancellation) => {
                var teacherExists = await _teacherRepository.GetAsync(creatorId);
                return teacherExists != null;
            }).WithMessage("CreatorId must exist in the Teacher repository.");
    }
}