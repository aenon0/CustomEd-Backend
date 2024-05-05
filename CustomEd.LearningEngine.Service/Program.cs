using System.Security.Claims;
using CustomEd.LearningEngine.Service.Model;
using CustomEd.LearningEngine.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8080");
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


// Add Swagger generation services
builder.Services.AddSwaggerGen(c =>
{
    // Define the Swagger document
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Define the security scheme (e.g., JWT Bearer token)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Define the security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});














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
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rideshare API V1");
   c.RoutePrefix = "swagger"; // This will set the swagger UI route to 'http://localhost:8080/swagger'
   c.DocExpansion(DocExpansion.None);
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
