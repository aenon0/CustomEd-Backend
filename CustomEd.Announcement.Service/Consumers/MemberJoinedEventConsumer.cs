using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ClassRoom> _classRoomRepository;

        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<ClassRoom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var classroom = _mapper.Map<ClassRoom>(context.Message.ClassroomId);
            classroom.MemberIds.Add(context.Message.StudentId);
            await _classRoomRepository.UpdateAsync(classroom);

        }
    }
}