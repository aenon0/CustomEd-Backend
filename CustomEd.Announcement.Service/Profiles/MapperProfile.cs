using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.Model;
using CustomEd.Contracts.Classroom.Events;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Model.Announcement, AnnouncementDto>().ReverseMap();
            CreateMap<Model.Announcement, CreateAnnouncementDto>().ReverseMap();
            CreateMap<Model.Announcement, UpdateAnnouncementDto>().ReverseMap();

            CreateMap<ClassRoom, ClassroomCreatedEvent>().ReverseMap();
            CreateMap<ClassRoom, ClassroomUpdatedEvent>().ReverseMap();

        }
    }
}