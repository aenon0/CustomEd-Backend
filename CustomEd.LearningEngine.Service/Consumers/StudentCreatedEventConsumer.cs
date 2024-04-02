using System.Threading.Tasks;
using MassTransit;
using CustomEd.User.Contracts;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.LearningEngine.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<StudentCreatedEvent>
    {
        private readonly IGenericRepository<Model.Student> _studentRepository;
        private readonly IMapper _mapper;

        public StudentCreatedEventConsumer(IGenericRepository<Model.Student> studentRepository, IMapper mapper)
        {
    
            _studentRepository = studentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
        {
            var studentCreatedEvent = context.Message;
            var student = _mapper.Map<Model.Student>(studentCreatedEvent);
            student.Id = studentCreatedEvent.Id; 
            await _studentRepository.CreateAsync(student);
        }
    }
}