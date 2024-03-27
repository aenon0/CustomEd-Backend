using AutoMapper;
using CustomEd.Contracts.Announcements.Events;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;

namespace CustomEd.Notification.Service.Consumers.AnnouncementConsumers;

public class AnnouncementDeletedEventConsumer : IConsumer<AnnouncementDeletedEvent>
{
    private readonly IGenericRepository<Models.Announcement> _announcementRepository;
    private readonly IGenericRepository<Models.StudentAnnouncement> _studentAnnouncementRepository;
    public AnnouncementDeletedEventConsumer(IGenericRepository<Models.Announcement> announcementRepository, IGenericRepository<Models.StudentAnnouncement> studentAnnouncementRepository)
    {
        _announcementRepository = announcementRepository;
        _studentAnnouncementRepository = studentAnnouncementRepository;
    }

    public async Task Consume(ConsumeContext<AnnouncementDeletedEvent> context)
    {
        var announcementId = context.Message.Id;
        await _announcementRepository.RemoveAsync(announcementId);
        var studentAnnouncements = await _studentAnnouncementRepository.GetAllAsync(x => x.AnnouncementId == announcementId);
        foreach (var studentAnnouncement in studentAnnouncements)
        {
            await _studentAnnouncementRepository.RemoveAsync(studentAnnouncement);
        }
    }
}
