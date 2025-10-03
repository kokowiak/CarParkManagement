using Microsoft.Extensions.Options;
using Moq;

namespace CarParkManagement.Core.Tests;

public class ChargeCalculatorTests
{
    // TODO: should be extended to include more types in test data

    const decimal AdditionalChargeForTests = 1.5m;
    const decimal StandardChargeForTestType = 0.15m;
    const string TestCarType = "TestType";

    private readonly Mock<IOptions<ChargesConfiguration>> _chargesConfigurationMock;

    public ChargeCalculatorTests()
    {
        _chargesConfigurationMock = new Mock<IOptions<ChargesConfiguration>>();

        _chargesConfigurationMock.Setup(x => x.Value).Returns(new ChargesConfiguration
        {
            AdditionalCharge = AdditionalChargeForTests,
            ChargesPerMinute = new Dictionary<string, decimal>
            {
                { TestCarType, StandardChargeForTestType },
            }
        });
    }

    [Fact]
    public void CalculateCharge_ShouldThrowArgumentException_WhenLeftAtEarlierThanParkedAt()
    {
        // act && assert
        Assert.Throws<ArgumentException>(() =>
            CreateSut().CalculateCharge(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), TestCarType));
    }

    [Fact]
    public void CalculateCharge_ShouldThrowInvalidOperationException_WhenMissingChargeConfigForVehicleType()
    {
        // act && assert
        Assert.Throws<InvalidOperationException>(() =>
            CreateSut().CalculateCharge(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(4), $"Missing_Car_Type"));
    }

    [Fact]
    public void CalculateCharge_ShouldReturn4BaseCharges_WhenCarParkedFor3AndAHalvMinutes()
    {
        // arrange
        var parkedAt = DateTime.UtcNow;
        var leftAt = parkedAt.AddSeconds(3.5 * 60);

        // act
        var charge = CreateSut().CalculateCharge(parkedAt, leftAt, TestCarType);

        // assert
        Assert.Equal((double)(4 * StandardChargeForTestType), charge);
    }

    [Fact]
    public void CalculateCharge_ShouldReturn5BaseCharges_WhenCarParkedFor5Minutes()
    {
        // arrange
        var parkedAt = DateTime.UtcNow;
        var leftAt = parkedAt.AddSeconds(5 * 60);

        // act
        var charge = CreateSut().CalculateCharge(parkedAt, leftAt, TestCarType);

        // assert
        Assert.Equal((double)(5 * StandardChargeForTestType), charge);
    }

    [Fact]
    public void CalculateCharge_ShouldReturn6BaseChargesPlus1AdditionalCharge_WhenCarParkedForJustOver5Minutes()
    {
        // arrange
        var parkedAt = DateTime.UtcNow;
        var leftAt = parkedAt.AddSeconds(5.01 * 60);

        // act
        var charge = CreateSut().CalculateCharge(parkedAt, leftAt, TestCarType);

        // assert
        Assert.Equal((double)(6 * StandardChargeForTestType + AdditionalChargeForTests), charge);
    }

    private ChargeCalculator CreateSut() => new (_chargesConfigurationMock.Object);
}
