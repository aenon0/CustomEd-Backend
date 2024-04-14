using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Discussion.Service.Model;

namespace CustomEd.Discussion.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Message> _discussionRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Message> discussionRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _discussionRepository = discussionRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<Classroom>(context.Message);
            await _classRoomRepository.UpdateAsync(classroom);
            var discussions = await _discussionRepository.GetAllAsync(a => a.ClassroomId == classroom.Id);
            foreach (var discussion in discussions)
            {
                discussion.ClassroomId = classroom.Id;
                await _discussionRepository.UpdateAsync(discussion);
            }
        }
    }
}