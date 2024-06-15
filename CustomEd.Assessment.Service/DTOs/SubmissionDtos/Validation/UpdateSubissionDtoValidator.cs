using System;
using System.Collections.Generic;
using CustomEd.Assessment.Service.DTOs;
using FluentValidation;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Assessment.Service.DTOs.Validation;
public class UpdateSubmissionDtoValidator : AbstractValidator<UpdateSubmissionDto>
{
    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Answer> _answerRepository;
    private readonly IGenericRepository<Submission> _submissionRepository;  

    public UpdateSubmissionDtoValidator(
        IGenericRepository<Model.Assessment> assessmentRepository,
        IGenericRepository<Answer> answerRepository,
        IGenericRepository<Submission> submissionRepository

    )
    {
        _assessmentRepository = assessmentRepository;
        _answerRepository = answerRepository;
        _submissionRepository = submissionRepository;

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("StudentId is required.")
            .MustAsync(async (dto, studentId, cancellationToken) =>
            {
                var assessment = await _assessmentRepository.GetAsync(dto.AssessmentId);
                return assessment != null && assessment.Classroom.Members.Any(s => s == studentId);
                
            })
            .WithMessage("Student is not a member of the classroom.");
            
        RuleFor(x => x.AssessmentId)
            .NotEmpty()
            .WithMessage("AssessmentId is required.")
            .MustAsync(async (dto, assessmentId, cancellationToken) =>
            {
                var assessment = await _assessmentRepository.GetAsync(assessmentId);
                return assessment != null;
            })
            .WithMessage("Assessment does not exist.");

        RuleFor(x => x.Answers).NotEmpty().WithMessage("Answers is required.");
        RuleForEach(x => x.Answers)
            .NotEmpty()
            .WithMessage("AnswerId in Answers list cannot be empty.")
            .MustAsync(async (dto, answerId, cancellationToken) =>
            {
                var answer = await _answerRepository.GetAsync(answerId);
                return answer != null;
            })
            .WithMessage("AnswerId does not exist in the repository.");
        
        RuleFor(x =>  x.Answers.Count)
            .MustAsync(async (dto, answers, cancellationToken) =>
            {
                var assessment = await _assessmentRepository.GetAsync(dto.AssessmentId);
                return answers == assessment.Questions.Count;
            }).WithMessage("Number of answers must be equal to the number of questions in the assessment.");
    }
}
