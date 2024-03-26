using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository, IGenericRepository<Teacher> teacherRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<ClassRoom>(context.Message);
            await _classRoomRepository.UpdateAsync(classroom);
        }
    }
}