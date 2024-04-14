using AutoMapper;
using CustomEd.LearningEngine.Service.Model;
using CustomEd.User.Student.Events;

namespace CustomEd.LearningEngine.Service.Profiles;

public class MappingProfile : Profile
{       
    public MappingProfile()
    {
        CreateMap<StudentCreatedEvent, Student>().ReverseMap();    
    } 
    
}
