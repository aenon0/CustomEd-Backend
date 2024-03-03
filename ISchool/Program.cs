using ISchool.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var configuration = builder.Configuration; 
var connectionString = configuration.GetConnectionString("MongoConnectionString"); 
builder.Services.AddSingleton<IMongoClient>(sp => 
{ 
    return new MongoClient(connectionString); 
}); 
 
 
var databaseName = configuration.GetValue<string>("MongoDatabaseName"); 
 
builder.Services.AddScoped(sp => 
{ 
    var mongoClient = sp.GetRequiredService<IMongoClient>(); 
    var database = mongoClient.GetDatabase(databaseName); 
    var collectionNames = database.ListCollectionNames().ToList(); 
    if (!collectionNames.Contains("Student")) 
    { 
        database.CreateCollection("Student"); 
    }; 
    if (!collectionNames.Contains("Teacher")) 
    { 
        database.CreateCollection("Teacher"); 
    };
    return database; 
});

builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<TeacherRepository>();



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
