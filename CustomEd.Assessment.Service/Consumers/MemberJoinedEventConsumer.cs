using MassTransit;
using AutoMapper;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Assessment.Service.Model;
using CustomEd.Contracts.Classroom.Events;

namespace CustomEd.Assessment.Service.Consumers
{
    public class MemberJoinedEventConsumer : IConsumer<MemberJoinedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
        public MemberJoinedEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Assessment> assessmentRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _assessmentRepository = assessmentRepository;
        }

        public async Task Consume(ConsumeContext<MemberJoinedEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            if(classroom.Members == null) classroom.Members = new List<Guid>();
            classroom.Members.Add(context.Message.StudentId);
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