using CustomEd.Classroom.Service.Model;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo();
builder.Services.AddPersistence<Classroom>("Classroom");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransitWithRabbitMQ();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IdentityProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnlyPolicy", policy =>
        policy.Requirements.Add(new TeacherOnlyRequirement()));
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
