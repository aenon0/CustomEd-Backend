using CustomEd.Discussion.Service;
using CustomEd.Discussion.Service.Model;
using CustomEd.Discussion.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddMongo()
                .AddPersistence<Classroom>("Classroom")
                .AddPersistence<Message>("Message")
                .AddPersistence<Student>("Student")
                .AddPersistence<Teacher>("Teacher");
                
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransitWithRabbitMQ();
builder.Services.AddAuth();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IdentityProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MemberOnlyPolicy", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreatorOnlyPolicy", policy =>
        policy.Requirements.Add(new CreatorOnlyRequirment()));
});
builder.Services.AddScoped<IAuthorizationHandler, CreatorOnlyPolicy>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
