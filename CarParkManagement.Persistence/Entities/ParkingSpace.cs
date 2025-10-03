using System.ComponentModel.DataAnnotations;

namespace CarParkManagement.Persistence.Entities;

public sealed class ParkingSpace
{
    public int Id { get; set; }
    public string? ParkedVehicleReg { get; set; }
    public DateTime? ParkedAtUtc { get; set; }
    // TODO: consider enum
    public string? VehicleType { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
