using System.Text.Json.Serialization;
using CarParkManagement.Core;
using CarParkManagement.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: I don't know anything about authentication and authorization requirements
//app.UseAuthorization();

// TODO: add global exception handler middleware

// TODO: add healthchecks

app.MapControllers();

// Just for this home task, normally applied in CI/CD pipeline
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarParkManagementDbContext>();
    try
    {
        Console.WriteLine("Starting DB setup");
        db.Database.Migrate();
        Console.WriteLine("Database setup successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database setup failed: {ex.Message}");
        throw;
    }
}

app.Run();
