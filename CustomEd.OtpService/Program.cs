using Swashbuckle.AspNetCore.SwaggerUI;
using CustomEd.OtpService.Repository;
using CustomEd.OtpService.Service;
using MongoDB.Driver;
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

builder.WebHost.UseUrls("http://*:9090");

builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OtpRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

 
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
    if (!collectionNames.Contains("Otp"))
    {
        // Create the collection if it doesn't exist
        database.CreateCollection("Otp");
    };
    return database;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
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
