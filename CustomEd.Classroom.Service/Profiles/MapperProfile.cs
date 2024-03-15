using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Classroom.Service.Model;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Classroom, ClassroomDto>().ReverseMap();
    }
}