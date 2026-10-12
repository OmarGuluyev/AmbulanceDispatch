namespace AmbulanceDispatch.Api.Models;

public record Ambulance(
    int Id,
    string CallSign,
    AmbulanceStatus Status,
    string[] Equipment
);