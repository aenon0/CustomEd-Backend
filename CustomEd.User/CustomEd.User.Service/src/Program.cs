using CustomEd.Shared.Data;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Password;
using CustomEd.User.Service.PasswordService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register IConfiguration with the service collection
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddMongo();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddAuth();

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