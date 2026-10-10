var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Temporary ambulance data for our first prototype.
var ambulances = new List<Ambulance>
{
    new(
        1,
        "AMB-001",
        "Available",
        new[] { "Basic Life Support", "Oxygen" }
    ),
    new(
        2,
        "AMB-002",
        "Busy",
        new[] { "Advanced Life Support", "Oxygen" }
    ),
    new(
        3,
        "AMB-003",
        "Available",
        new[] { "Advanced Life Support", "Oxygen", "Defibrillator" }
    )
};

// GET /api/ambulances
app.MapGet("/api/ambulances", () =>
{
    return Results.Ok(ambulances);
});

app.Run();

record Ambulance(
    int Id,
    string CallSign,
    string Status,
    string[] Equipment
);
