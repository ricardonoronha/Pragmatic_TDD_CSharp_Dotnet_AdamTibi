namespace Uqs.Weather.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
