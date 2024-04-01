using AutoMapper;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper.QueryableExtensions;
using CustomEd.Contracts.Classroom.Events;

namespace CustomEd.Assessment.Service.Profiles;
public class MappingProfile : Profile
{
    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Classroom> _classroomRepository;

    public MappingProfile(IGenericRepository<Model.Assessment> assessmentRepository, IGenericRepository<Classroom> classroomRepository)
    {
        

        _assessmentRepository = assessmentRepository;
        _classroomRepository = classroomRepository;

        CreateMap<CreateQuestionDto, Question>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.Select(a => new Answer { Text = a, })));
            

        CreateMap<UpdateQuestionDto, Question>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.Select(a => new Answer { Text = a, })));

        CreateMap<Answer, AnswerDto>();

        CreateMap<Question, QuestionDto>()
        .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.Select(a => a.Text)));

        CreateMap<CreateAssessmentDto, Model.Assessment>()
            .ForMember(dest => dest.Classroom, opt => opt.MapFrom(async (src, dest, destMember, context) => await _classroomRepository.GetAsync(src.ClassroomId)));
        
        CreateMap<UpdateAssessmentDto, Model.Assessment>()
            .ForMember(dest => dest.Classroom, opt => opt.MapFrom(async (src, dest, destMember, context) => await _classroomRepository.GetAsync(src.ClassroomId)));
        
        CreateMap<Classroom, ClassroomDto>().ReverseMap();
        
        CreateMap<Model.Assessment, AssessmentDto>()
            .ForMember(dest => dest.Classroom, opt => opt.MapFrom(src => src.Classroom));
        
        CreateMap<Submission, SubmissionDto>().ReverseMap();
        CreateMap<CreateSubmissionDto, Submission>();
        CreateMap<UpdateSubmissionDto, Submission>();
        
        CreateMap<Analytics, AnalyticsDto>()
            .ForMember(dest => dest.Assessment, opt => opt.MapFrom(src => src.Assessment));
        
        CreateMap<ClassroomCreatedEvent, Classroom>().ReverseMap();
        CreateMap<ClassroomUpdatedEvent, Classroom>().ReverseMap();

        
        
            

      
    }
}