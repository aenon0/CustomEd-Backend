using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
    {
        private readonly IGenericRepository<Student> _StudentRepository;
        private readonly IMapper _mapper;

        public StudentDeletedEventConsumer(IGenericRepository<Student> StudentRepository, IMapper mapper)
        {
    
            _StudentRepository = StudentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentDeletedEvent> context)
        {
            var StudentDeletedEvent = context.Message; 
            await _StudentRepository.RemoveAsync(StudentDeletedEvent.Id);
            return;
        }
    }
}