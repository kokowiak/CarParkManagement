namespace CarParkManagement.Core;

internal interface IDateTimeProvider
{
    DateTime GetUtcNow();
}
