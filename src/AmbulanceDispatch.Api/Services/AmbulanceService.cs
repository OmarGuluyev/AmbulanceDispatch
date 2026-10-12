using AmbulanceDispatch.Api.Models;

namespace AmbulanceDispatch.Api.Services;

public class AmbulanceService
{
    private readonly List<Ambulance> _ambulances = new()
    {
        new(
            1,
            "AMB-001",
            AmbulanceStatus.Available,
            new[] { "Basic Life Support", "Oxygen" }
        ),
        new(
            2,
            "AMB-002",
            AmbulanceStatus.OnCall,
            new[] { "Advanced Life Support", "Oxygen" }
        ),
        new(
            3,
            "AMB-003",
            AmbulanceStatus.Available,
            new[] { "Advanced Life Support", "Oxygen", "Defibrillator" }
        )
    };

    public List<Ambulance> GetAll()
    {
        return _ambulances;
    }

public Ambulance? GetById(int id)
{
    return _ambulances.FirstOrDefault(a => a.Id == id);
}

    public bool UpdateStatus(int id, AmbulanceStatus status)
    {
        var index = _ambulances.FindIndex(a => a.Id == id);

        if (index == -1)
        {
            return false;
        }

        var ambulance = _ambulances[index];

        _ambulances[index] = ambulance with { Status = status };

        return true;
    }
}