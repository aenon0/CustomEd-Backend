using CustomEd.Discussion.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using CustomEd.Discussion.Service.DTOs;

namespace CustomEd.Discussion.Service.DTOs.Validtion;
public class CreateMessageDtoValidator : AbstractValidator<CreateMessageDto>
{
    private readonly IGenericRepository<Classroom> _classRoomRepositry;
    private readonly IGenericRepository<Student> _studentRepositry;
    public CreateMessageDtoValidator(IGenericRepository<Classroom> classRoomRepositry, IGenericRepository<Student> studentRepositry)
    {
        _classRoomRepositry = classRoomRepositry;
        _studentRepositry = studentRepositry;

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters");
       
        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("ClassRoomId is required")
            .MustAsync(async (id, token) => {
                var classroom = await _classRoomRepositry.GetAsync(id);
                return classroom != null;
            }).WithMessage("ClassRoomId does not exist");
        
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required")
            .MustAsync(async (id, token) => {
                var classroom = await _classRoomRepositry.GetAsync(id);
                return classroom != null;
            }).WithMessage("SenderId does not exist");
    }
}
