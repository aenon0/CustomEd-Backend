using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Notification.Service.Models;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Notification.Service.Consumers.StudentConsumers;
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