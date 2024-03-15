using System.Threading.Tasks;
using MassTransit;
using CustomEd.Classroom.Service.Consumers;
using CustomEd.User.Contracts;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    public class TeacherCreatedEventConsumer : IConsumer<TeacherCreatedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherCreatedEventConsumer(IGenericRepository<Teacher> teacherRepository, IMapper mapper)
        {
    
            _teacherRepository = teacherRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<TeacherCreatedEvent> context)
        {
            var teacherCreatedEvent = context.Message;
            var teacher = _mapper.Map<Teacher>(teacherCreatedEvent);
            teacher.Id = teacherCreatedEvent.Id; 
            await _teacherRepository.CreateAsync(teacher);
            return;
        }
    }
}