using AutoMapper;
using CustomEd.Contracts.Announcements.Events;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;


namespace CustomEd.Notification.Service.Consumers.AnnouncementConsumers;
 
public class AnnouncementCreatedEventConsumer : IConsumer<AnnouncementCreatedEvent>
{
    private readonly IGenericRepository<Models.Announcement> _announcementRepository;
    private readonly IGenericRepository<Models.StudentAnnouncement> _studentAnnouncementRepository;
    private readonly IGenericRepository<ClassRoom> _classRoomRepository;
    private readonly IMapper _mapper;
    public AnnouncementCreatedEventConsumer(IGenericRepository<Models.Announcement> announcementRepository, IGenericRepository<Models.StudentAnnouncement> studentAnnouncementRepository, IGenericRepository<ClassRoom> classRoomRepository, IMapper mapper)
    {
        _announcementRepository = announcementRepository;
        _studentAnnouncementRepository = studentAnnouncementRepository;
        _classRoomRepository = classRoomRepository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AnnouncementCreatedEvent> context)
    {
        var announcement = _mapper.Map<Models.Announcement>(context.Message);
        var classRoomId = announcement.ClassRoomId;
        var classRoom = await _classRoomRepository.GetAsync(x => x.Id == classRoomId);
        var studentIds = classRoom.MemberIds;
        foreach (var studentId in studentIds)
        {
            var studentAnnouncement = new Models.StudentAnnouncement
            {
                Id = Guid.NewGuid(),
                AnnouncementId = announcement.Id,
                StudentId = studentId
            };
            await _studentAnnouncementRepository.CreateAsync(studentAnnouncement);
        }
        await _announcementRepository.CreateAsync(announcement);
    }
}
