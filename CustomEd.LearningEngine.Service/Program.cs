using System.Security.Claims;
using CustomEd.LearningEngine.Service.Model;
using CustomEd.LearningEngine.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddMongo();
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddPersistence<Student>("LearningPath");
builder.Services.AddPersistence<Student>("ChatbotMesage");

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddAuth();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IdentityProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnlyPolicy", policy =>
        policy.Requirements.Add(new StudentOnlyRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, StudentOnlyPolicy>();
builder.Services.AddControllers();


builder.Services.AddControllers();
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
