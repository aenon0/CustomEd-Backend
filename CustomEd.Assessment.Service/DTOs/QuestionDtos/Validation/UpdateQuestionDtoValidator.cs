using System;
using System.Collections.Generic;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;

namespace CustomEd.Assessment.Service.DTOs.Validation;
public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
{
    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Answer> _answerRepository;
    private readonly IGenericRepository<Question> _questionRepository;

    public UpdateQuestionDtoValidator(
        IGenericRepository<Model.Assessment> assessmentRepository,
        IGenericRepository<Answer> answerRepository, 
        IGenericRepository<Question> questionRepository
    )
    {
        _assessmentRepository = assessmentRepository;
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;

        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Id can not be null")
            .MustAsync(
                async (questionId, cancellation) =>
                {
                    var question = await _questionRepository.GetAsync(questionId);
                    return question != null;
                }
            )
            .WithMessage("Question with given id does not exist.");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required.")
            .Length(0, 500)
            .WithMessage("Question text cannot be more than 500 characters.");
        
        RuleFor(x => x.Weight)
            .GreaterThan(0.0)
            .WithMessage("Weight must be greater than or equal to 0.");

        RuleFor(x => x.Answers).NotEmpty().WithMessage("At least one answer is required.");

        RuleFor(x => x.CorrectAnswerIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Correct index must be greater than or equal to 0.")
            .LessThan(x => x.Answers.Count)
            .WithMessage("Correct index must be less than the number of answers.");

        RuleFor(x => x.AssessmentId)
            .NotEmpty()
            .WithMessage("Assessment ID is required.")
            .MustAsync(
                async (assessmentId, cancellation) =>
                {
                    var assessment = await _assessmentRepository.GetAsync(assessmentId);
                    return assessment != null;
                }
            )
            .WithMessage("No such assessment id exists in the database.");

        RuleFor(x => x.Tags).NotEmpty().WithMessage("At least one tag is required.");
    }
}