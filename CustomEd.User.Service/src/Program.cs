using CustomEd.Shared.Data;
using CustomEd.Shared.Settings;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Password;
using CustomEd.User.Service.PasswordService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using CustomEd.User.Service.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using CustomEd.Shared.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });
builder.WebHost.UseUrls("http://*:5000");
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(nameof(MongoSettings)));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection(nameof(ServiceSettings)));


builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddControllers();
builder.Services.AddMongo();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddPersistence<Admin>("Admin");
builder.Services.AddPersistence<Otp>("Otp");
builder.Services.AddPersistence<ForgotPasswordOtp>("ForgotPasswordOtp");
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddAuth();
builder.Services.AddAuthorization();

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
app.UseCors("AllowAll");
app.UseSwagger();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rideshare API V1");
    c.RoutePrefix = "swagger";
    c.DocExpansion(DocExpansion.None);
});


app.UseAuthorization();

app.MapControllers();

app.Run();




