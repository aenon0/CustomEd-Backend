using Microsoft.AspNetCore.SignalR;

namespace CustomEd.Notification.Service.Hubs;

public class AssessmentHub : Hub
{
      public override async Task OnConnectedAsync()
    {
        var clientId = Context.GetHttpContext().Request.Query["clientId"].ToString();
        Console.WriteLine(clientId);
        await base.OnConnectedAsync();
        await SendAnnouncment(Guid.Empty, Guid.Empty);
    }
     
    public async Task SendAnnouncment(Guid TeacherId, Guid ClassroomId)
    {
        await Clients.All.SendAsync("ReceiveAnnouncement", TeacherId, ClassroomId, "Announcement note");
        Console.WriteLine($"Annoucement Sent!");
    }
    public async Task SendMessageToClassroomMembers(Guid ClassroomId, string message)
    {
        //to be ammended base on the above function 
        // Fetch group members from the database
        // List<string> groupMembers = await _groupMembershipService.GetGroupMembers(groupName);

        // Send message to each group member
        // foreach (var member in groupMembers)
        // {
        //     await Clients.User(member).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        // }
    }


    //what i can call for the client side for marking
    public async Task MarkAnnouncementAsSeen(Guid AnnouncementId)
    {
        Console.WriteLine($"Mark the announcement as seen for the client");
    }
}
