namespace CarParkManagement.Core.Dtos;

public sealed record VehicleChargeDto(string VehicleReg, double VehicleCharge, DateTime TimeIn, DateTime TimeOut);
