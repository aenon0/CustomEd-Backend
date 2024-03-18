using CustomEd.Shared.Data.Interfaces;
using FluentValidation;
using System;

namespace CustomEd.Classroom.Service.DTOs.Validation
{
    public class AddStudentDtoValidator : AbstractValidator<AddStudentDto>
    {
        private IGenericRepository<Model.Student> _studentRepository;
        private IGenericRepository<Model.Classroom> _classroomRepository;
        public AddStudentDtoValidator(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository)
        {
            _studentRepository = studentRepository;
            _classroomRepository = classroomRepository;
    
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    return await _studentRepository.GetAsync(id) != null;
                }).WithMessage("Student with such id does not exist.");

            RuleFor(x => new {sid = x.StudentId,cid =  x.ClassroomId})
                .MustAsync(async (dto, cancellation) =>
                {
                    var classroom = await _classroomRepository.GetAsync(x => x.Id == dto.cid);
                    return classroom.Members.Where(x => x.Id == dto.sid).FirstOrDefault() == null;
                }).WithMessage("Student is already in the classroom.");
        }
    }
}