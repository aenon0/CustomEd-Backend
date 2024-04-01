using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Assessment.Service.Model;

namespace CustomEd.Assessment.Service.Consumers
{
    public class ClassroomCreatedEventConsumer : IConsumer<ClassroomCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;

        public ClassroomCreatedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomCreatedEvent> context)
        {
            var classroom = _mapper.Map<Classroom>(context.Message);
            await _classRoomRepository.CreateAsync(classroom);

        }
    }
}