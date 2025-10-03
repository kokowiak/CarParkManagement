using CarParkManagement.Core.Dtos;

namespace CarParkManagement.Core;

public interface ICarParkManagementService
{
    // TODO: introduce result pattern?
    Task<SpacesInfoDto> GetSpacesInfoAsync(CancellationToken cancellationToken);
    Task<ParkedVehicleDto?> ParkVehicleAsync(VehicleToBeParkedDto vehicleToBeParkedDto);
    Task<VehicleChargeDto?> ChargeVehicleAsync(VehicleToBeChargedDto vehicleToLeaveDto);
}
