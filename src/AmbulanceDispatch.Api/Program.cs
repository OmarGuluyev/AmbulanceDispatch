using AmbulanceDispatch.Api.Services;
using AmbulanceDispatch.Api.Models;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AmbulanceService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// GET /api/ambulances
app.MapGet("/api/ambulances", (AmbulanceService service) =>
{
    return Results.Ok(service.GetAll());
});


app.MapPut("/api/ambulances/{id:int}/status",
    (int id, string status, AmbulanceService service) =>
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

    var updated = service.UpdateStatus(id, parsedStatus);

    if (!updated)
    {
        return Results.NotFound(new
        {
            message = "Ambulance not found"
        });
    }

    return Results.Ok(new
    {
        message = "Ambulance status updated"
    });
});


app.MapGet("/api/ambulances/{id:int}", (int id, AmbulanceService service) =>
{
    var ambulance = service.GetById(id);

    if (ambulance is null)
    {
        return Results.NotFound(new { message = "Ambulance not found" });
    }

    return Results.Ok(ambulance);
});

app.Run();


