using System.Data;
using CarParkManagement.Core.Dtos;
using CarParkManagement.Persistence;
using CarParkManagement.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarParkManagement.Core;

internal sealed class CarParkManagementService : ICarParkManagementService
{
    private readonly CarParkManagementDbContext _carParkManagementDbContext;
    private readonly IChargeCalculator _chargeCalculator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger _logger;

    public CarParkManagementService(
        CarParkManagementDbContext carParkManagementDbContext,
        IChargeCalculator chargeCalculator,
        IDateTimeProvider dateTimeProvider,
        ILogger<CarParkManagementService> logger)
    {
        _carParkManagementDbContext = carParkManagementDbContext;
        _chargeCalculator = chargeCalculator;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<VehicleChargeDto?> ChargeVehicleAsync(VehicleToBeChargedDto vehicleToLeaveDto)
    {
        var leftAtUtc = _dateTimeProvider.GetUtcNow();

        // TODO: consider using ValueType for vehicle reg with trimming on creation

        try
        {
            ParkingSpace? spaceDetails = await _carParkManagementDbContext.ParkingSpaces.SingleOrDefaultAsync(
                ps => ps.ParkedVehicleReg == vehicleToLeaveDto.VehicleReg.Trim());

            if (spaceDetails == null)
            {
                return null;
            }

            var parkedAtUtc = spaceDetails.ParkedAtUtc!.Value;
            double charge = _chargeCalculator.CalculateCharge(spaceDetails.ParkedAtUtc!.Value, leftAtUtc, spaceDetails.VehicleType!);

            spaceDetails.ParkedVehicleReg = null;
            spaceDetails.ParkedAtUtc = null;
            spaceDetails.VehicleType = null;

            // TODO: decide how charging should work, what to do if e.g. charge calculation failed, clear the space or not

            await _carParkManagementDbContext.SaveChangesAsync();

            return new VehicleChargeDto(vehicleToLeaveDto.VehicleReg.Trim(), charge, parkedAtUtc, leftAtUtc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while charging vehicle with reg {VehicleReg}", vehicleToLeaveDto.VehicleReg);
            throw;
        }
    }

    public async Task<SpacesInfoDto> GetSpacesInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            List<bool> arePlacesAvailableInfos = await _carParkManagementDbContext.ParkingSpaces
                .Select(ps => ps.ParkedVehicleReg == null)
                .ToListAsync(cancellationToken);

            var availableCount = arePlacesAvailableInfos.Count(isAvailable => isAvailable);
            var occupiedCount = arePlacesAvailableInfos.Count - availableCount;

            return new SpacesInfoDto(availableCount, occupiedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting spaces info");
            throw;
        }
    }

    public async Task<ParkedVehicleDto?> ParkVehicleAsync(VehicleToBeParkedDto vehicleToBeParkedDto)
    {
        // TODO: check if car with same reg plate is not already parked (add unique constraint in db on not null vehicle reg)

        try
        {
            ParkingSpace? firstFreeSpot = await _carParkManagementDbContext.ParkingSpaces
                .OrderBy(ps => ps.Id)
                .FirstOrDefaultAsync(ps => ps.ParkedVehicleReg == null);

            if (firstFreeSpot == null)
            {
                return null;
            }

            firstFreeSpot.ParkedVehicleReg = vehicleToBeParkedDto.VehicleReg.Trim();
            firstFreeSpot.ParkedAtUtc = _dateTimeProvider.GetUtcNow();
            // TODO: consider using enum in the entity as well + mapper
            firstFreeSpot.VehicleType = vehicleToBeParkedDto.VehicleType.ToString();

            await _carParkManagementDbContext.SaveChangesAsync();

            return new ParkedVehicleDto(firstFreeSpot.ParkedVehicleReg, firstFreeSpot.Id, firstFreeSpot.ParkedAtUtc!.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while parking vehicle with reg {VehicleReg}", vehicleToBeParkedDto.VehicleReg);
            throw;
        }
    }
}
