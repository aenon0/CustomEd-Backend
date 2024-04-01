using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Assessment.Service.Model;

namespace CustomEd.Assessment.Service.Consumers
{
    public class ClassroomUpdatedEventConsumer : IConsumer<ClassroomUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;

        public ClassroomUpdatedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Assessment> assessmentRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _assessmentRepository = assessmentRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomUpdatedEvent> context)
        {
            var classroom = _mapper.Map<Classroom>(context.Message);
            await _classRoomRepository.UpdateAsync(classroom);
            var assessments = await _assessmentRepository.GetAllAsync(a => a.Classroom.Id == classroom.Id);
            foreach (var assessment in assessments)
            {
                assessment.Classroom = classroom;
                await _assessmentRepository.UpdateAsync(assessment);
            }
        }
    }
}