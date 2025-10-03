using Microsoft.Extensions.Options;

namespace CarParkManagement.Core;

internal sealed class ChargeCalculator : IChargeCalculator
{
    // TODO: should charges be moved to db or some config provider like Azure AppConfiguration?
    private readonly ChargesConfiguration _chargesConfiguration;

    public ChargeCalculator(IOptions<ChargesConfiguration> chargesConfiguration)
    {
        _chargesConfiguration = chargesConfiguration.Value;
    }

    public double CalculateCharge(DateTime parkedAt, DateTime leftAt, string vehicleType)
    {
        if (leftAt < parkedAt)
        {
            throw new ArgumentException("leftAt cannot be earlier than parkedAt", nameof(leftAt));
        }

        // TODO: consider switching to Strategy pattern
        if (!_chargesConfiguration.ChargesPerMinute.TryGetValue(vehicleType, out decimal chargePerMinute))
        {
            // TODO: Swith to result pattern?
            throw new InvalidOperationException($"Cannot find charge for vehicle type: {vehicleType}");
        }

        var totalMinutes = (int)Math.Ceiling((leftAt - parkedAt).TotalMinutes);

        // TODO: confirm if this is correct way to apply additional charge (i.e. after every 5 minutes)
        var additionalCharge = _chargesConfiguration.AdditionalCharge * ((totalMinutes - 1)/5);
        // TODO: confirm if this is correct way to apply standard charge (i.e. every started minute)
        var baseCharge = chargePerMinute * totalMinutes;

        // TODO: shouldn't we return decimal in API?
        return (double)(baseCharge + additionalCharge);
    }
}
