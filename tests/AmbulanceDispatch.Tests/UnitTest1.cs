using AmbulanceDispatch.Api.Data;
using AmbulanceDispatch.Api.Models;
using AmbulanceDispatch.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AmbulanceDispatch.Tests;

public class AmbulanceServiceTests
{
 
private static AmbulanceDbContext CreateDbContext()
{
    var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
    connection.Open();

    var options = new DbContextOptionsBuilder<AmbulanceDbContext>()
        .UseSqlite(connection)
        .Options;

    var db = new AmbulanceDbContext(options);
    db.Database.EnsureCreated();

    return db;
}


    [Fact]
    public async Task GetAllAsync_ReturnsAllAmbulances()
    {
        await using var db = CreateDbContext();

        db.Ambulances.AddRange(
            new Ambulance
            {
                CallSign = "AMB-001",
                Status = AmbulanceStatus.Available,
                Equipment = new[] { "Oxygen" }
            },
            new Ambulance
            {
                CallSign = "AMB-002",
                Status = AmbulanceStatus.OnCall,
                Equipment = new[] { "Defibrillator" }
            });

        await db.SaveChangesAsync();

        var service = new AmbulanceService(db);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAmbulanceExists_ReturnsAmbulance()
    {
        await using var db = CreateDbContext();

        var ambulance = new Ambulance
        {
            CallSign = "AMB-001",
            Status = AmbulanceStatus.Available,
            Equipment = new[] { "Oxygen" }
        };

        db.Ambulances.Add(ambulance);
        await db.SaveChangesAsync();

        var service = new AmbulanceService(db);
        var result = await service.GetByIdAsync(ambulance.Id);

        Assert.NotNull(result);
        Assert.Equal("AMB-001", result.CallSign);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAmbulanceDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var service = new AmbulanceService(db);
        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenAmbulanceExists_UpdatesStatus()
    {
        await using var db = CreateDbContext();

        var ambulance = new Ambulance
        {
            CallSign = "AMB-001",
            Status = AmbulanceStatus.Available,
            Equipment = new[] { "Oxygen" }
        };

        db.Ambulances.Add(ambulance);
        await db.SaveChangesAsync();

        var service = new AmbulanceService(db);
        var result = await service.UpdateStatusAsync(
            ambulance.Id,
            AmbulanceStatus.OnCall);

        Assert.NotNull(result);
        Assert.Equal(AmbulanceStatus.OnCall, result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenAmbulanceDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var service = new AmbulanceService(db);
        var result = await service.UpdateStatusAsync(
            999,
            AmbulanceStatus.Available);

        Assert.Null(result);
    }


[Fact]
public async Task FindSuitableAsync_RequiresAllEquipment()
{
    // Arrange: create three ambulances
    await using var db = CreateDbContext();

    db.Ambulances.AddRange(
        new Ambulance
        {
            CallSign = "AMB-001",
            Status = AmbulanceStatus.Available,
            Equipment = new[] { "Oxygen" }
        },
        new Ambulance
        {
            CallSign = "AMB-002",
            Status = AmbulanceStatus.Dispatched,
            Equipment = new[] { "Oxygen", "Ventilator" }
        },
        new Ambulance
        {
            CallSign = "AMB-003",
            Status = AmbulanceStatus.Available,
            Equipment = new[] { "Oxygen", "Ventilator" }
        });

    await db.SaveChangesAsync();

    var service = new AmbulanceService(db);

    // Act: request an ambulance with both items
    var result = await service.FindSuitableAsync(
        new[] { "Oxygen", "Ventilator" });

    // Assert: only AMB-003 meets both requirements
    Assert.NotNull(result);
    Assert.Equal("AMB-003", result.CallSign);
}



[Fact]
public async Task DispatchAsync_WhenAmbulanceIsAvailable_DispatchesIt()
{
    using var db = CreateDbContext();

    var ambulance = new Ambulance
    {
        CallSign = "AMB-001",
        Status = AmbulanceStatus.Available,
        Equipment = new[] { "Oxygen" }
    };

    db.Ambulances.Add(ambulance);
    await db.SaveChangesAsync();

    var service = new AmbulanceService(db);

    var result = await service.DispatchAsync(ambulance.Id);
    await db.Entry(ambulance).ReloadAsync();

    Assert.NotNull(result);
    Assert.Equal(AmbulanceStatus.Dispatched, result.Status);
}

[Fact]
public async Task DispatchAsync_WhenAmbulanceDoesNotExist_ReturnsNull()
{
    using var db = CreateDbContext();
    var service = new AmbulanceService(db);

    var result = await service.DispatchAsync(999);

    Assert.Null(result);
}

[Fact]
public async Task DispatchAsync_WhenAmbulanceIsNotAvailable_ReturnsNull()
{
    using var db = CreateDbContext();

    var ambulance = new Ambulance
    {
        CallSign = "AMB-002",
        Status = AmbulanceStatus.OnCall,
        Equipment = new[] { "Oxygen" }
    };

    db.Ambulances.Add(ambulance);
    await db.SaveChangesAsync();

    var service = new AmbulanceService(db);

    var result = await service.DispatchAsync(ambulance.Id);

    Assert.Null(result);
    Assert.Equal(AmbulanceStatus.OnCall, ambulance.Status);
}


}