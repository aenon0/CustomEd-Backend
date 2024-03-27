using AutoMapper;
using CustomEd.Contracts.Announcements.Events;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;

namespace CustomEd.Notification.Service.Consumers.AnnouncementConsumers;

public class AnnouncementUpdatedEventConsumer : IConsumer<AnnouncementUpdatedEvent>
{

    private readonly IGenericRepository<Models.Announcement> _announcementRepository;
    private readonly IMapper _mapper;
    public AnnouncementUpdatedEventConsumer(IGenericRepository<Models.Announcement> announcementRepository, IMapper mapper)
    {
        _announcementRepository = announcementRepository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AnnouncementUpdatedEvent> context)
    {
        var announcement = _mapper.Map<Models.Announcement>(context.Message);
        await _announcementRepository.UpdateAsync(announcement);
    }
}
