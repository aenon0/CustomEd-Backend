using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;
using AutoMapper;
using CustomEd.User.Teacher.Events;
using CustomEd.User.Contracts.Teacher.Events;

namespace CustomEd.Discussion.Service.Consumers
{
    public class TeacherDeletedEventConsumer : IConsumer<TeacherDeletedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;

        public TeacherDeletedEventConsumer(IGenericRepository<Teacher> teacherRepository)
        {
    
            _teacherRepository = teacherRepository;
        }
        public async Task Consume(ConsumeContext<TeacherDeletedEvent> context)
        {
            var teacherDeletedEvent = context.Message; 
            var deletedTeacher = await _teacherRepository.GetAsync(teacherDeletedEvent.Id);
            deletedTeacher.isDeleted = true;
            await _teacherRepository.UpdateAsync(deletedTeacher);
        }
    }
}