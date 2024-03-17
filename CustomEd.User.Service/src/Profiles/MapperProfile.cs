// Purpose: This file contains the mapping profiles for the user service.
using AutoMapper;
using CustomEd.User.Service.DTOs;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;

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
        CreateMap<Model.Teacher,  TeacherUpdatedEvent>().ReverseMap();

        CreateMap<Model.Student,  StudentCreatedEvent>().ReverseMap();
        CreateMap<Model.Student,  StudentUpdatedEvent>().ReverseMap();



    }

}