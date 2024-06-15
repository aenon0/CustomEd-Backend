using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;
using System;

namespace CustomEd.Assessment.Service.DTOs.Validation;
public class UpdateAssessmentDtoValidator : AbstractValidator<UpdateAssessmentDto>
{
    private readonly IGenericRepository<Classroom> _classroomRepository;
    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;

    public UpdateAssessmentDtoValidator(IGenericRepository<Model.Assessment> assessmentRepository, IGenericRepository<Classroom>  classroomRepository)
    {
        _classroomRepository = classroomRepository;
        _assessmentRepository = assessmentRepository;

        RuleFor(x => x.Id).NotNull().WithMessage("Id can not be null")
                          .MustAsync(async (assessmentId, cancellation) => 
                                   {
                                       var assessment = await _assessmentRepository.GetAsync(assessmentId);
                                       return assessment != null;
                                   }).WithMessage("Assessment with given id does not exist.")
                          .MustAsync(async (assessmentId, cancellation) => 
                                   {
                                       var assessment = await _assessmentRepository.GetAsync(assessmentId);
                                       return !assessment.IsPublished;
                                   }).WithMessage("Published assessments is already published.");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.")
                                   .Length(1, 500).WithMessage("Description must be between 1 and 500 characters.");

        RuleFor(x => x.Tag).NotEmpty().WithMessage("Tag is required.");

        RuleFor(x => x.ClassroomId).NotEmpty().WithMessage("ClassroomId is required.")
                                   .MustAsync(async (classroomId, cancellation) => 
                                   {
                                       var classroom = await _classroomRepository.GetAsync(classroomId);
                                       return classroom != null;
                                   }).WithMessage("Classroom with given id does not exist.");

        RuleFor(x => x.Deadline).NotEmpty().WithMessage("Deadline is required.")
                                .GreaterThan(DateTime.Now).WithMessage("Deadline must be in the future.");
    }
}