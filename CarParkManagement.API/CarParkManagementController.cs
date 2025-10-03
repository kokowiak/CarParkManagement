using CarParkManagement.Core;
using CarParkManagement.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarParkManagement.API;

[ApiController]
[Route("parking")]
public sealed class CarParkManagementController : ControllerBase
{
    private readonly ICarParkManagementService _carParkManagementService;

    public CarParkManagementController(
        ICarParkManagementService carParkManagementService)
    {
        _carParkManagementService = carParkManagementService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpacesInfoDto))]
    public async Task<IActionResult> GetSpacesInfoAsync(CancellationToken cancellationToken)
    {
        return Ok(await _carParkManagementService.GetSpacesInfoAsync(cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParkedVehicleDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ParkVehicleAsync(VehicleToBeParkedDto vehicleToBeParked)
    {
        // TODO: consider fluent validation
        if (string.IsNullOrWhiteSpace(vehicleToBeParked.VehicleReg))
        {
            return BadRequest("Vehicle registration cannot be empty");
        }

        var parkedVehicleDto = await _carParkManagementService.ParkVehicleAsync(vehicleToBeParked);

        // TODO: for now assume null means no space available, consider introducing result pattern later
        if (parkedVehicleDto == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(parkedVehicleDto);
        }
    }

    [HttpPost()]
    [Route("exit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleChargeDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChargeVehicleAsync(VehicleToBeChargedDto vehicleToBeCharged)
    {
        // TODO: consider fluent validation
        if (string.IsNullOrWhiteSpace(vehicleToBeCharged.VehicleReg))
        {
            return BadRequest("Vehicle registration cannot be empty");
        }

        var vehicleChargeDto = await _carParkManagementService.ChargeVehicleAsync(vehicleToBeCharged);

        // TODO: for now assume null means no car with given plate found, consider introducing result pattern later
        if (vehicleChargeDto == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(vehicleChargeDto);
        }
    }       
}
