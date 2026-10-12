
namespace AmbulanceDispatch.Api.Models;

public class Ambulance
{
    public int Id { get; set; }

    public string CallSign { get; set; } = string.Empty;

    public AmbulanceStatus Status { get; set; }

    public string[] Equipment { get; set; } = Array.Empty<string>();
}
