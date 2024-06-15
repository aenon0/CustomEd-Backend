using AutoMapper;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper.QueryableExtensions;
using CustomEd.Contracts.Classroom.Events;

namespace CustomEd.Assessment.Service.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateMap<CreateQuestionDto, Question>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.Select(a => new Answer { Text = a, })));
            

        CreateMap<UpdateQuestionDto, Question>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.Select(a => new Answer { Text = a, })));

        CreateMap<Answer, AnswerDto>();

        CreateMap<Question, QuestionDto>()
        .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

        CreateMap<CreateAssessmentDto, Model.Assessment>();
        
        CreateMap<UpdateAssessmentDto, Model.Assessment>();
        
        CreateMap<Classroom, ClassroomDto>().ReverseMap();
        
        CreateMap<Model.Assessment, AssessmentDto>()
            .ForMember(dest => dest.Classroom, opt => opt.MapFrom(src => src.Classroom));
        
        CreateMap<Submission, SubmissionDto>().ReverseMap();
        CreateMap<CreateSubmissionDto, Submission>();
        CreateMap<UpdateSubmissionDto, Submission>();
        
        CreateMap<Analytics, AnalyticsDto>()
            .ForMember(dest => dest.Assessment, opt => opt.MapFrom(src => src.Assessment));
        
        CreateMap<ClassroomCreatedEvent, Classroom>().ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.MemberIds));
        CreateMap<ClassroomUpdatedEvent, Classroom>().ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.MemberIds));

            

      
    }
}