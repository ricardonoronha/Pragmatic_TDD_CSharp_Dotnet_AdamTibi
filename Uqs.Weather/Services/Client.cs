using Newtonsoft.Json;
using System.Globalization;
using System.Web;

namespace Uqs.Weather.Services
{
    public class Client : IClient
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5";

        public Client(string apiKey, HttpClient httpClient)
        {
            _apiKey = apiKey;
            _httpClient = httpClient;
        }

        public async Task<WeatherResponse> WeatherCallAsync(decimal latitude, decimal longitude, Units unit)
        {
            const string WEATHER_URL_TEMPLATE = "/forecast";
            var uriBuilder = new UriBuilder(BASE_URL + WEATHER_URL_TEMPLATE);
            var query = HttpUtility.ParseQueryString("");
            query["lat"] = latitude.ToString(CultureInfo.InvariantCulture);
            query["lon"] = longitude.ToString(CultureInfo.InvariantCulture);
            query["appid"] = _apiKey;
            query["units"] = unit.ToString().ToLower();
            uriBuilder.Query = query.ToString();

            var jsonResponse = await _httpClient.GetStringAsync(uriBuilder.Uri.AbsoluteUri);
            if (string.IsNullOrEmpty(jsonResponse))
            {
                throw new InvalidOperationException("No response from the service");
            }

            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(jsonResponse)!;

            return weatherResponse;
        }
    }
}
