using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;

namespace CustomEd.Discussion.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Discussion> _discussionRepository;
        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Discussion> discussionRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _discussionRepository = discussionRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var classroom = _mapper.Map<Classroom>(context.Message.ClassroomId);
            classroom.Members.Add(context.Message.StudentId);
            await _classRoomRepository.UpdateAsync(classroom);
            var discussions = await _discussionRepository.GetAllAsync(a => a.Classroom.Id == classroom.Id);
            foreach (var discussion in discussions)
            {
                discussion.Classroom = classroom;
                await _discussionRepository.UpdateAsync(discussion);
            }

        }
    }
}