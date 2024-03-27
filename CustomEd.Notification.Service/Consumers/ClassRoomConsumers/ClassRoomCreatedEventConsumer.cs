using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;

namespace CustomEd.Notification.Service.Consumers.ClassRoomConsumers;

public class ClassRoomCreatedEventConsumer : IConsumer<ClassroomCreatedEvent>
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<ClassRoom> _classRoomRepository;
    public ClassRoomCreatedEventConsumer(IGenericRepository<ClassRoom> classRoomRepository, IMapper mapper)
    {
        _classRoomRepository = classRoomRepository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
    {
        var classroom = _mapper.Map<ClassRoom>(context.Message);
        await _classRoomRepository.CreateAsync(classroom);
    }
} 
