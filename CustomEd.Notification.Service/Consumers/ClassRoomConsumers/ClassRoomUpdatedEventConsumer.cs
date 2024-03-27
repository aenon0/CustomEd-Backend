using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;

namespace CustomEd.Notification.Service.Consumers.ClassRoomConsumers;

public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
{
    private readonly IGenericRepository<ClassRoom> _classRoomRepository;
    private readonly IMapper _mapper;
    
    public ClassroomUpdatedEventConsumer(IGenericRepository<ClassRoom> classRoomRepository, IMapper mapper)
    {
        _classRoomRepository = classRoomRepository;
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
    {
        var classroom = _mapper.Map<ClassRoom>(context.Message);
        await _classRoomRepository.UpdateAsync(classroom);
    }
}
