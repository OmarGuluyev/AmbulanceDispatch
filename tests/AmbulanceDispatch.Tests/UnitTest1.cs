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
        var options = new DbContextOptionsBuilder<AmbulanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AmbulanceDbContext(options);
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
}