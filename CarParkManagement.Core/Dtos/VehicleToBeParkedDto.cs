namespace CarParkManagement.Core.Dtos;

// TODO: any restrictions on VehicleReg length?
public sealed record VehicleToBeParkedDto(string VehicleReg, VehicleType VehicleType);
