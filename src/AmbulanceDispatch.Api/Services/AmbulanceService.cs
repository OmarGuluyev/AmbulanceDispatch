
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
}
