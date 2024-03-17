using FluentValidation;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;

namespace CustomEd.Classroom.Service.DTOs.Validation;
public class UpdateClassroomDtoValidator : AbstractValidator<UpdateClassroomDto>
{
    private readonly IGenericRepository<Teacher> _teacherRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;

    public UpdateClassroomDtoValidator(IGenericRepository<Teacher> teacherRepository, IGenericRepository<Model.Classroom> classroomRepository)
    {
        _teacherRepository = teacherRepository;
        _classroomRepository = classroomRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.")
            .MustAsync(async (id, cancellation) => {
                var classroomExists = await _classroomRepository.GetAsync(id);
                return classroomExists != null;
            }).WithMessage("Id must exist in the Classroom repository.");

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