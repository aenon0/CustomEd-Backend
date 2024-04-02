using CustomEd.Discussion.Service.Model;
using CustomEd.Discussion.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMongo()
                .AddPersistence<Discussion>("Classroom")
                .AddPersistence<Discussion>("Discussion");

                
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MemberOnlyPolicy", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();



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
