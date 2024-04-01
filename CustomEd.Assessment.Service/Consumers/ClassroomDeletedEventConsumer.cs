using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Assessment.Service.Model;

namespace CustomEd.Assessment.Service.Consumers
{
    public class ClassroomDeletedEventConsumer : IConsumer<ClassroomDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;

        public ClassroomDeletedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Assessment> assessmentRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _assessmentRepository = assessmentRepository;
        }

        public async Task Consume(ConsumeContext<ClassroomDeletedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.Id);
            if (classroom == null)
            {
                return;
            }
            await _classRoomRepository.RemoveAsync(classroom);
            var assessments = await _assessmentRepository.GetAllAsync(a => a.Classroom.Id == classroom.Id);
            foreach (var assessment in assessments)
            {
                await _assessmentRepository.RemoveAsync(assessment);
            }


        }
    }
}