using System.Security.Claims;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.Notification.Service.Hubs;

public class AnnouncementHub : Hub
{
    // private readonly IGenericRepository<AnnoucementItem> _annoucementRepository;
     private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public AnnouncementHub(IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        // _annoucementRepository = annoucementRepository;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            // var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            // var userId = identityProvider.GetUserId();
            // if(userId == null)
            // {
            //     return;
            // }

            Console.WriteLine("been here");
            await Clients.Caller.SendAsync("Connected", $"Welcome to the AnnouncementHub! You're the user with {token} token.");
            
            ///fetch the unread annoucements using the user id
            ///send unread announcements for the caller 
        }

        await base.OnConnectedAsync();
    }
        
    public async Task MarkAnnouncementAsSeen(Guid AnnouncementId)
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var userId = identityProvider.GetUserId();
            //mark the announcement read on the database for the user 
        }
    }

}
