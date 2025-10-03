using CarParkManagement.Persistence;
using CarParkManagement.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarParkManagement.Core.Tests;

// TODO: consider using fluent assertions or alternative, since fluent assertions are now paid :(

public class CarParkManagementServiceTests
{
    private const string TestParkedCarReg = "TEST_REG";
    private const string TestCarType = "TEST_CAR_TYPE";

    private readonly Mock<ILogger<CarParkManagementService>> _loggerMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IChargeCalculator> _chargeCalculatorMock;

    private readonly DateTime TestCarParkedAt = DateTime.UtcNow;

    public CarParkManagementServiceTests()
    {
        _loggerMock = new Mock<ILogger<CarParkManagementService>>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _chargeCalculatorMock = new Mock<IChargeCalculator>();
    }

    [Fact]
    public async Task GetSpacesInfoAsync_ShouldReturnCorrectNumberOfAvailableAndOccupiedPlaces_WhenCarParked()
    {
        // arrange
        const int TotalNumberOfSpaces = 3;
        using var dbContext = SetupParkingSpacesDb(TotalNumberOfSpaces);

        // act
        var result = await CreateSut(dbContext).GetSpacesInfoAsync();

        // assert
        Assert.Equal(TotalNumberOfSpaces - 1, result.AvailableSpaces);
        Assert.Equal(1, result.OccupiedSpaces);
    }

    [Fact]
    public async Task ChargeVehicleAsync_ShouldReturnNull_WhenCannotFindACarByReg()
    {
        // arrange
        using var dbContext = SetupParkingSpacesDb(totalNumberOfSpaces: 1);

        var vehicleToBeCharged = new Dtos.VehicleToBeChargedDto("NOT_FOUND_REG");

        // act
        var result = await CreateSut(dbContext).ChargeVehicleAsync(vehicleToBeCharged);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ChargeVehicleAsync_ShouldClearTheParkingSpace_WhenCarFoundAndChargeCalculated()
    {
        // arrange
        using var dbContext = SetupParkingSpacesDb(totalNumberOfSpaces: 2);

        var parkingSpaceId = dbContext.ParkingSpaces
            .First(x => x.ParkedVehicleReg == TestParkedCarReg)
            .Id;

        var vehicleToBeCharged = new Dtos.VehicleToBeChargedDto(TestParkedCarReg);

        // act
        var result = await CreateSut(dbContext).ChargeVehicleAsync(vehicleToBeCharged);

        // assert
        var clearedParkingSpace = dbContext.ParkingSpaces.Find(parkingSpaceId);
        Assert.Null(clearedParkingSpace!.ParkedVehicleReg);
        Assert.Null(clearedParkingSpace!.ParkedAtUtc);
        Assert.Null(clearedParkingSpace!.VehicleType);
    }

    [Fact]
    public async Task ChargeVehicleAsync_ShouldReturnChargeCalculatedByChargeCalculator_WhenCarFoundAndChargeSuccessfullyCalculated()
    {
        // arrange
        using var dbContext = SetupParkingSpacesDb(totalNumberOfSpaces: 2);
        var vehicleToBeCharged = new Dtos.VehicleToBeChargedDto(TestParkedCarReg);

        var expectedCharge = 10.5;

        var leftAt = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(x => x.GetUtcNow()).Returns(leftAt);
        _chargeCalculatorMock.Setup(x => x.CalculateCharge(TestCarParkedAt, leftAt, TestCarType)).Returns(expectedCharge);

        // act
        var result = await CreateSut(dbContext).ChargeVehicleAsync(vehicleToBeCharged);

        // assert
        Assert.NotNull(result);
        Assert.Equal(expectedCharge, result.VehicleCharge);
    }

    // TODO: make it more configurable
    private CarParkManagementDbContext SetupParkingSpacesDb(int totalNumberOfSpaces)
    {
        var options = new DbContextOptionsBuilder<CarParkManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CarParkManagementDbContext(options);

        context.ParkingSpaces.Add(new ParkingSpace 
        { 
            Id = 1, 
            ParkedVehicleReg = TestParkedCarReg,
            ParkedAtUtc = TestCarParkedAt,
            VehicleType = TestCarType,
            RowVersion = [0]
        });

        for (int i = 2; i <= totalNumberOfSpaces; i++)
        {
            context.ParkingSpaces.Add(new ParkingSpace { Id = i, RowVersion = [0] });
        }

        context.SaveChanges();

        return context;
    }

    private CarParkManagementService CreateSut(CarParkManagementDbContext dbContext) => 
        new(dbContext, _chargeCalculatorMock.Object, _dateTimeProviderMock.Object, _loggerMock.Object);
}
