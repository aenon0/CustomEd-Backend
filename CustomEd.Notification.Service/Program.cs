using System.Text;
using CustomEd.Notification.Service.Hubs;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers();
builder.Services.AddMongo();
builder.Services.AddPersistence<ClassRoom>("ClassRoom");
builder.Services.AddPersistence<Announcement>("Announcement");
builder.Services.AddPersistence<StudentAnnouncement>("StudentAnnouncement");
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();


var app = builder.Build();

app.UseRouting();
app.UseAuthentication(); 


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AnnouncementHub>("/getAnnouncement");
});

await app.RunAsync();
