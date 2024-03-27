using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;

namespace CustomEd.Announcement.Service.DTOs.Validtion;
public class CreateAnnouncementDtoValidator : AbstractValidator<CreateAnnouncementDto>
{
    private readonly IGenericRepository<ClassRoom> _classRoomRepositry;
    public CreateAnnouncementDtoValidator(IGenericRepository<ClassRoom> classRoomRepositry)
    {
        _classRoomRepositry = classRoomRepositry;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters");
        
        RuleFor(x => x.Attachments)
            .NotEmpty().WithMessage("Attachments is required")
            .Must(x => x.Count <= 5).WithMessage("Attachments must not exceed 5 items");
        
        RuleFor(x => x.ClassRoomId)
            .NotEmpty().WithMessage("ClassRoomId is required")
            .MustAsync(async (id, token) => {
                var classroom = await _classRoomRepositry.GetAsync(id);
                return classroom != null;
            }).WithMessage("ClassRoomId does not exist");
        
        RuleFor(x => x.TimeStamp)
            .NotEmpty().WithMessage("TimeStamp is required");
    }
}