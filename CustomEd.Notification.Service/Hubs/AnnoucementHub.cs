using System.Security.Claims;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.Notification.Service.Hubs;

public class AnnouncementHub : Hub
{
    private readonly IGenericRepository<Models.Announcement> _annoucementRepository;
    private readonly IGenericRepository<Models.StudentAnnouncement> _studentAnnoucementRepository;
     private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public AnnouncementHub(IGenericRepository<Models.Announcement> annoucementRepository, IGenericRepository<Models.StudentAnnouncement> studentAnnoucementRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _annoucementRepository = annoucementRepository;
        _studentAnnoucementRepository = studentAnnoucementRepository;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var studentId = identityProvider.GetUserId();
            if (studentId == Guid.Empty)
            {
                return;
            }
            var studentAnnouncements = await _studentAnnoucementRepository.GetAllAsync(x => x.StudentId == studentId && x.IsRead == false);
            foreach (var studentAnnouncement in studentAnnouncements)
            {
                var announcementId = studentAnnouncement.AnnouncementId;
                var annoucement = await _annoucementRepository.GetAsync(announcementId);
                await Clients.Caller.SendAsync("Announcement", annoucement);
            }
            Console.WriteLine("done sending all the announcement");
        }

        await base.OnConnectedAsync();
    }
        
    public async Task MarkAnnouncementAsSeen(Guid announcementId)
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var studentId = identityProvider.GetUserId();
            var studentAnnoucement = await _studentAnnoucementRepository.GetAsync(x => x.StudentId == studentId && x.AnnouncementId == announcementId);
            if(studentAnnoucement != null)
            {
                studentAnnoucement.IsRead = true;
                await _studentAnnoucementRepository.UpdateAsync(studentAnnoucement);
            }
        }
    }

}
