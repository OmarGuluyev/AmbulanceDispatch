
using System.Linq;
using AmbulanceDispatch.Api.Data;
using AmbulanceDispatch.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceDispatch.Api.Services;

public class AmbulanceService
{
    private readonly AmbulanceDbContext _db;

    public AmbulanceService(AmbulanceDbContext db)
    {
        _db = db;
    }

    public async Task<List<Ambulance>> GetAllAsync()
    {
        return await _db.Ambulances.ToListAsync();
    }

    public async Task<Ambulance?> GetByIdAsync(int id)
    {
        return await _db.Ambulances.FindAsync(id);
    }

    public async Task<Ambulance?> UpdateStatusAsync(
        int id,
        AmbulanceStatus status)
    {
        var ambulance = await _db.Ambulances.FindAsync(id);

        if (ambulance is null)
        {
            return null;
        }

        ambulance.Status = status;

        await _db.SaveChangesAsync();

        return ambulance;
    }



public async Task<Ambulance?> DispatchAsync(int id)
{
    var rowsAffected = await _db.Ambulances
        .Where(a => a.Id == id &&
                    a.Status == AmbulanceStatus.Available)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(
                a => a.Status,
                AmbulanceStatus.Dispatched));

    if (rowsAffected == 0)
    {
        return null;
    }

    return await _db.Ambulances.FindAsync(id);
}



    

public async Task<Ambulance?> FindSuitableAsync(
    string[] requiredEquipment)
{
    var availableAmbulances = await _db.Ambulances
        .Where(a => a.Status == AmbulanceStatus.Available)
        .OrderBy(a => a.Id)
        .ToListAsync();

    return availableAmbulances.FirstOrDefault(ambulance =>
        requiredEquipment.All(required =>
            ambulance.Equipment.Contains(
                required,
                StringComparer.OrdinalIgnoreCase)));
}


}
