using AutoMapper;
using CustomEd.Announcement.Service.DTOs;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Model.Announcement, AnnouncementDto>().ReverseMap();
            CreateMap<Model.Announcement, CreateAnnouncementDto>().ReverseMap();
            CreateMap<Model.Announcement, UpdateAnnouncementDto>().ReverseMap();
        }
    }
}