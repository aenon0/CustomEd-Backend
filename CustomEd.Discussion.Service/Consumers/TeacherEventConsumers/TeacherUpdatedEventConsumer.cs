using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.Discussion.Service.Consumers
{
    ///I think you forgot to replace Teacher with teacher
    public class TeacherUpdatedEventConsumer : IConsumer<TeacherUpdatedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherUpdatedEventConsumer(IGenericRepository<Teacher> teacherRepository, IMapper mapper)
        {
    
            _teacherRepository = teacherRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<TeacherUpdatedEvent> context)
        {
            var TeacherUpdatedEvent = context.Message;
            var Teacher = _mapper.Map<Teacher>(TeacherUpdatedEvent);
            Teacher.Id = TeacherUpdatedEvent.Id; 
            await _teacherRepository.UpdateAsync(Teacher);
            return;
        }
    }
}