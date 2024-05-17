using ISchool.Model;
using ISchool.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8080");

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
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
    if (!collectionNames.Contains("DepartmentCourses"))
    {
        database.CreateCollection("DepartmentCourses");
    };
    return database;
});

builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<DepartmentCourses>();
builder.Services.AddScoped<TeacherRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rideshare API V1");
    c.RoutePrefix = "swagger"; // This will set the swagger UI route to 'http://localhost:8080/swagger'
    c.DocExpansion(DocExpansion.None);
});


app.UseAuthorization();

app.MapControllers();

app.Run();
