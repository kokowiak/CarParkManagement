namespace CarParkManagement.Core.Dtos;

public sealed record ParkedVehicleDto(string VehicleReg, int SpaceNumber, DateTime TimeIn);
