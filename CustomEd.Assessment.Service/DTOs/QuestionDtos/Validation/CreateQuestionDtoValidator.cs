using System;
using System.Collections.Generic;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;

namespace CustomEd.Assessment.Service.DTOs.Validation;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Answer> _answerRepository;

    public CreateQuestionDtoValidator(
        IGenericRepository<Model.Assessment> assessmentRepository,
        IGenericRepository<Answer> answerRepository
    )
    {
        _assessmentRepository = assessmentRepository;
        _answerRepository = answerRepository;

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required.")
            .Length(0, 500)
            .WithMessage("Question text cannot be more than 500 characters.");

        RuleFor(x => x.Answers).NotEmpty().WithMessage("At least one answer is required.");

        RuleFor(x => x.CorrectAnswerId)
            .NotEmpty()
            .WithMessage("Correct answer ID is required.")
            .MustAsync(
                async (correctAnswerId, cancellation) =>
                {
                    var answer = await _answerRepository.GetAsync(correctAnswerId);
                    return answer != null;
                }
            )
            .WithMessage("Correct answer does not exist in the database.");

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
