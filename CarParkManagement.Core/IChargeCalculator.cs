namespace CarParkManagement.Core;

internal interface IChargeCalculator
{
    double CalculateCharge(DateTime parkedAt, DateTime leftAt, string vehicleType);
}
