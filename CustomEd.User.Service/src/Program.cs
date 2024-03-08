using CustomEd.User.Service.Data;
using CustomEd.User.Service.Model;
using CustomEd.User.Service.Password;
using CustomEd.User.Service.PasswordService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMongo();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddPersistence<User>("Users");
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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
