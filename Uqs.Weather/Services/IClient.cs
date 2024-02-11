
namespace Uqs.Weather.Services
{
    public interface IClient
    {
        Task<WeatherResponse> WeatherCallAsync(decimal latitude, decimal longitude, Units unit);
    }
}