using System.Threading.Tasks;
using MassTransit;
using CustomEd.Discussion.Service.Consumers;
using CustomEd.User.Contracts;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Discussion.Service.Consumers
{
    public class StudentCreatedEventConsumer : IConsumer<StudentCreatedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IMapper _mapper;

        public StudentCreatedEventConsumer(IGenericRepository<Student> studentRepository, IMapper mapper)
        {
    
            _studentRepository = studentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
        {
            Console.WriteLine("BEEEEEN HERE");
            var StudentCreatedEvent = context.Message;
            var Student = _mapper.Map<Student>(StudentCreatedEvent);
            Student.Id = StudentCreatedEvent.Id; 
            await _studentRepository.CreateAsync(Student);
            return;
        }
    }
}