using AutoMapper;
using CustomEd.Contracts.Announcements.Events;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Notification.Service.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ClassRoom, ClassroomCreatedEvent>().ReverseMap();
            CreateMap<ClassRoom, ClassroomUpdatedEvent>().ReverseMap();
            
            CreateMap<Notification.Service.Models.Announcement, AnnouncementCreatedEvent>().ReverseMap();
            CreateMap<Notification.Service.Models.Announcement, AnnouncementUpdatedEvent>().ReverseMap();


        }
    }
}