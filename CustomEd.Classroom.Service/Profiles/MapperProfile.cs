using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Classroom.Service.Model;
using CustomEd.User.Contracts.Teacher.Events;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Classroom, ClassroomDto>().ReverseMap();

        CreateMap<TeacherCreatedEvent, Teacher>().ReverseMap();
        CreateMap<TeacherUpdatedEvent, Teacher>().ReverseMap();

        CreateMap<StudentCreatedEvent, Student>().ReverseMap();
        CreateMap<StudentUpdatedEvent, Student>().ReverseMap();
        
    }
}