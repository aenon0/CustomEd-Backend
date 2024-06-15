using MassTransit;
using AutoMapper;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Assessment.Service.Model;
using CustomEd.Contracts.Classroom.Events;

namespace CustomEd.Assessment.Service.Consumers
{
    public class MemberLeftEventConsumer : IConsumer<MemberLeftEvent>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Classroom> _classRoomRepository;
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
        public MemberLeftEventConsumer(IMapper mapper, IGenericRepository<Classroom> classRoomRepository, IGenericRepository<Model.Assessment> assessmentRepository)
        {
            _mapper = mapper;
            _classRoomRepository = classRoomRepository;
            _assessmentRepository = assessmentRepository;
        }

        public async Task Consume(ConsumeContext<MemberLeftEvent> context)
        {
            var classroom = await _classRoomRepository.GetAsync(context.Message.ClassroomId);
            classroom.Members = classroom.Members.Where(m => m != context.Message.StudentId).ToList();
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