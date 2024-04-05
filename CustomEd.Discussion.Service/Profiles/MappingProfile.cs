using AutoMapper;
using CustomEd.Discussion.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper.QueryableExtensions;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;

namespace CustomEd.Discussion.Service.Profiles;
public class MappingProfile : Profile
{
    private readonly IGenericRepository<Classroom> _classroomRepository;

    public MappingProfile(IGenericRepository<Classroom> classroomRepository)
    {
        CreateMap<ClassroomCreatedEvent, Classroom>().ReverseMap();
        CreateMap<ClassroomUpdatedEvent, Classroom>().ReverseMap();

        CreateMap<StudentCreatedEvent, Student>().ReverseMap();
        CreateMap<StudentUpdatedEvent, Student>().ReverseMap();

        CreateMap<TeacherCreatedEvent, Teacher>().ReverseMap();
        CreateMap<TeacherUpdatedEvent, Teacher>().ReverseMap();
    }
}