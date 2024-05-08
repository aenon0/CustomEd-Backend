using System.Security.Claims;
using CustomEd.Classroom.Service.Model;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo();
builder.Services.AddPersistence<Classroom>("Classroom");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddAuth();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IdentityProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnlyPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, CustomEd.Shared.Model.Role.Teacher.ToString());
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


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
