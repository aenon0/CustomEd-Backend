using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Notification.Service.Models;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.Notification.Service.Consumers.StudentConsumers;

public class TeacherUpdatedEventConsumer : IConsumer<TeacherUpdatedEvent>
{
private readonly IGenericRepository<Student> _studentRepository;
private readonly IMapper _mapper;

public TeacherUpdatedEventConsumer(IGenericRepository<Student> studentRepository, IMapper mapper)
{

    _studentRepository = studentRepository;
    _mapper = mapper;   
}
public async Task Consume(ConsumeContext<TeacherUpdatedEvent> context)
{
    var teacherUpdatedEvent = context.Message;
    var teacher = _mapper.Map<Student>(teacherUpdatedEvent);
    teacher.Id = teacherUpdatedEvent.Id; 
    await _studentRepository.UpdateAsync(teacher);
    return;
}
}
