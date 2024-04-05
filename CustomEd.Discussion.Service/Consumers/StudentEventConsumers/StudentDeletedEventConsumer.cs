using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Discussion.Service.Consumers
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
            var deletedStudent = await _studentRepository.GetAsync(StudentDeletedEvent.Id);
            deletedStudent.isDeleted = true;
            await _studentRepository.UpdateAsync(deletedStudent);
        }
    }
}