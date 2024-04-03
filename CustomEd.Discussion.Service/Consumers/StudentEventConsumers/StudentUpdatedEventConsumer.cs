using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Discussion.Service.Consumers
{
    ///I think you forgot to replace Student with student
    public class StudentUpdatedEventConsumer : IConsumer<StudentUpdatedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IMapper _mapper;

        public StudentUpdatedEventConsumer(IGenericRepository<Student> studentRepository, IMapper mapper)
        {
    
            _studentRepository = studentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentUpdatedEvent> context)
        {
            var studentUpdatedEvent = context.Message;
            var student = _mapper.Map<Student>(studentUpdatedEvent);
            student.Id = studentUpdatedEvent.Id; 
            await _studentRepository.UpdateAsync(student);
        }
    }
}