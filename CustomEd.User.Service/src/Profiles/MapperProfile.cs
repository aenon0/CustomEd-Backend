// Purpose: This file contains the mapping profiles for the user service.
using AutoMapper;
using CustomEd.User.Contracts;
using CustomEd.User.Service.DTOs;

namespace CustomEd.User.Service.Profiles.MapperProfile;

public class MappingProfile: Profile{
    public MappingProfile()
    {
        CreateMap<Model.Student, CreateStudentDto>().ReverseMap();
        CreateMap<Model.Student, StudentDto>().ReverseMap();
        CreateMap<Model.Student, UpdateStudentDto>().ReverseMap();

        CreateMap<Model.Teacher,  CreateTeacherDto>().ReverseMap();
        CreateMap<Model.Teacher,  TeacherDto>().ReverseMap();
        CreateMap<Model.Teacher,  UpdateTeacherDto>().ReverseMap();

        CreateMap<Model.Teacher,  TeacherCreatedEvent>().ReverseMap();


    }

}