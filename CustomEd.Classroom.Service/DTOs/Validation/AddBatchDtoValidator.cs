using FluentValidation;
using CustomEd.Shared.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Classroom.Service.DTOs.Validation
{
    public class AddBatchDtoValidator : AbstractValidator<AddBatchDto>
    {
        private IGenericRepository<Model.Classroom> _classroomRepository;
        public AddBatchDtoValidator(IGenericRepository<Model.Classroom> classroomRepository)
        {
            _classroomRepository = classroomRepository;
            RuleFor(x => x.Section)
                .NotEmpty()
                .WithMessage("Section is required.");

            RuleFor(x => x.Year)
                .GreaterThan(0)
                .WithMessage("Year must be greater than 0.");

            RuleFor(x => x.Department)
                .NotNull()
                .WithMessage("Department is required.")
                .Must(x => Enum.IsDefined(typeof(Department), x))
                .WithMessage("Department must be a valid department.");
            
            RuleFor(x => x.ClassRoomId)
                .NotEmpty()
                .WithMessage("ClassRoomId is required.")
                .MustAsync(async (classRoomId, cancellation) => {
                    var classroomExists = await _classroomRepository.GetAsync(classRoomId);
                    return classroomExists != null;
                }).WithMessage("ClassRoomId must exist in the Classroom repository.");
        }
    }
}