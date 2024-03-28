using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongo()
                .AddPersistence<Answer>("Answers")
                .AddPersistence<Assessment>("Assessment")
                .AddPersistence<Classroom>("Classrooms")
                .AddPersistence<Question>("Questions")
                .AddPersistence<Submission>("Submissions");

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
