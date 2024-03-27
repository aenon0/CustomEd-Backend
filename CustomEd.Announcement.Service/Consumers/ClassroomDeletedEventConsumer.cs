using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Teacher> _teacherRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository, IGenericRepository<Teacher> teacherRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            await _teacherRepository.RemoveAsync(new Teacher { Id = classroom.CreatorId });

        }
    }
}