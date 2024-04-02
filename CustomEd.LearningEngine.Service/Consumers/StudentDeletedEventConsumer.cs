using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.User.Student.Events;
using CustomEd.LearningEngine.Service.Model;

namespace CustomEd.LearningEngine.Service.Consumers
{
    public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IMapper _mapper;

        public StudentDeletedEventConsumer(IGenericRepository<Student> studentRepository, IMapper mapper)
        {
    
            _studentRepository = studentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
        {
            var StudentDeletedEvent = context.Message; 
            await _studentRepository.RemoveAsync(StudentDeletedEvent.Id);
            return;
        }
    }
}