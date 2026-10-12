using AmbulanceDispatch.Api.Services;
using AmbulanceDispatch.Api.Data;
using AmbulanceDispatch.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AmbulanceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AmbulanceDatabase")));

        builder.Services.AddScoped<AmbulanceService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();

var app = builder.Build();


await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AmbulanceDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Ambulances.AnyAsync())
    {
        db.Ambulances.AddRange(
            new Ambulance
            {
                CallSign = "AMB-001",
                Status = AmbulanceStatus.Available,
                Equipment = new[] { "Basic Life Support", "Oxygen" }
            },
            new Ambulance
            {
                CallSign = "AMB-002",
                Status = AmbulanceStatus.OnCall,
                Equipment = new[] { "Advanced Life Support", "Oxygen" }
            },
            new Ambulance
            {
                CallSign = "AMB-003",
                Status = AmbulanceStatus.Available,
                Equipment = new[]
                {
                    "Advanced Life Support",
                    "Oxygen",
                    "Defibrillator"
                }
            }
        );

        await db.SaveChangesAsync();
    }
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/api/ambulances", async (AmbulanceService service) =>
{
    var ambulances = await service.GetAllAsync();

    return Results.Ok(ambulances);
});

app.MapGet("/api/ambulances/{id:int}",
    async (int id, AmbulanceService service) =>
{
    var ambulance = await service.GetByIdAsync(id);

    if (ambulance is null)
    {
        return Results.NotFound(new
        {
            message = "Ambulance not found"
        });
    }

    return Results.Ok(ambulance);
});

app.MapPut("/api/ambulances/{id:int}/status",
    async (int id, string status, AmbulanceService service) =>
{
    if (!Enum.TryParse<AmbulanceStatus>(
            status, true, out var parsedStatus)
        || !Enum.IsDefined(parsedStatus))
    {
        return Results.BadRequest(new
        {
            message = "Invalid status",
            allowedStatuses = Enum.GetNames<AmbulanceStatus>()
        });
    }

    var ambulance = await service.UpdateStatusAsync(
        id, parsedStatus);

    if (ambulance is null)
    {
        return Results.NotFound(new
        {
            message = "Ambulance not found"
        });
    }

    return Results.Ok(new
    {
        message = "Ambulance status updated",
        ambulance
    });
});


app.Run();
