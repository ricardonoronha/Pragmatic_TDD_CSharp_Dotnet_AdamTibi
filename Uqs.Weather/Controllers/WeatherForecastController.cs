using AdamTibi.OpenWeather;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uqs.Weather.Providers;

namespace Uqs.Weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const int FORECAST_DAYS = 5;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IConfiguration _config;

    private readonly ILogger<WeatherForecastController> _logger;

    public readonly IDateTimeProvider _dateTimeProvider;

    public readonly IRandomProvider _randomProvider;

    public readonly IClient _client;

    public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger, IDateTimeProvider dateTimeProvider, IRandomProvider randomProvider, IClient client)
    {
        _config = configuration;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _randomProvider = randomProvider;
        _client = client;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    [HttpGet("GetRandomWeatherForecast")]
    public IEnumerable<WeatherForecast> GetRandom()
    {
        WeatherForecast[] wfs = new WeatherForecast[FORECAST_DAYS];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new WeatherForecast();
            wf.Date = _dateTimeProvider.Now.AddDays(i + 1);
            wf.TemperatureC = _randomProvider.Next(-20, 55);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }


    [HttpGet("ConvertCToF")]
    public double ConvertCToF(double c)
    {
        double f = c * (9d / 5d) + 32;
        _logger.LogInformation("conversion requested");
        return f;
    }

    private string MapFeelToTemp(int temperatureC)
    {
        if (temperatureC <= 0) return Summaries.First();
        int summariesIndex = (temperatureC / 5) + 1;
        if (summariesIndex >= Summaries.Length) return
           Summaries.Last();
        return Summaries[summariesIndex];
    }

    [HttpGet("GetRealWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetReal()
    {
        // const decimal GREENWICH_LAT = 51.4810m;
        // const decimal GREENWICH_LON = 0.0052m;

        const decimal GREENWICH_LAT = -5.53394m; // novo oriente 
        const decimal GREENWICH_LON = -40.7751m; // novo oriente 
       
        OneCallResponse res = await _client.OneCallAsync(GREENWICH_LAT, GREENWICH_LON, new[] { Excludes.Current, Excludes.Minutely, Excludes.Hourly, Excludes.Alerts }, Units.Metric);
        WeatherForecast[] wfs = new WeatherForecast[FORECAST_DAYS];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new WeatherForecast();
            wf.Date = res.Daily[i + 1].Dt;
            double forecastedTemp = res.Daily[i + 1].Temp.Day;
            wf.TemperatureC = (int)Math.Round(forecastedTemp);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }
}
