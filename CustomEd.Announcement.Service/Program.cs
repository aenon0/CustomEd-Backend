using CustomEd.Announcement.Service.Model;
using CustomEd.Announcement.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMongo();
builder.Services.AddPersistence<ClassRoom>("ClassRooms");
builder.Services.AddPersistence<Teacher>("Teachers");
builder.Services.AddPersistence<Announcement>("Announcements");
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();


// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreatorOnlyPolicy", policy =>
        policy.Requirements.Add(new CreatorOnlyRequirement()));
    options.AddPolicy("MemberOnlyPolicy", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();
builder.Services.AddScoped<IAuthorizationHandler, CreatorOnlyPolicy>();
builder.Services.AddMassTransitWithRabbitMq();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
