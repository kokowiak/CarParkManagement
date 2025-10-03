namespace CarParkManagement.Core;

internal sealed record ChargesConfiguration
{
    public required Dictionary<string, decimal> ChargesPerMinute { get; init; }
    public required decimal AdditionalCharge { get; init; }
}
