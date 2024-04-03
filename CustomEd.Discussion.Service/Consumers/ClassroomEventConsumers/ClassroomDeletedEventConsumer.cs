using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;

namespace CustomEd.Discussion.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Discussion> _discussionRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Discussion> discussionRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _discussionRepository = discussionRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            var discussions = await _discussionRepository.GetAllAsync(a => a.Classroom.Id == classroom.Id);
            foreach (var discussion in discussions)
            {
                await _discussionRepository.RemoveAsync(discussion);
            }
        }
    }
}