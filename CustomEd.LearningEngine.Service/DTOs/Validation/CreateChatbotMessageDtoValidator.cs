using CustomEd.LearningEngine.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;

namespace CustomEd.LearningEngine.Service.DTOs.Validation;

public class CreateChatbotMessageDtoValidator : AbstractValidator<CreateLearningPathDto>
{
    private readonly IGenericRepository<Student> _studentRepository;
    public CreateChatbotMessageDtoValidator(IGenericRepository<Student> studentRepository)
    {
        _studentRepository = studentRepository;
        RuleFor(dto => dto.StudentId)
                .NotEmpty()
                .WithMessage("Student ID is required.");
        RuleFor(dto => dto.Content)
                .NotEmpty()
                .WithMessage("Content is required.");      
    }
}
