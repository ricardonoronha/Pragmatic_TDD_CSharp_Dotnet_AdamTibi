using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uqs.Weather.Providers;
using Uqs.Weather.Services;

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

        WeatherResponse res = await _client.WeatherCallAsync(GREENWICH_LAT, GREENWICH_LON, Units.Metric);

        var noon = TimeSpan.FromHours(12);

        IEnumerable<WeatherForecast> wfs = res.List
            .Where(x => x.Dt.TimeOfDay == noon)
            .Select(x => new WeatherForecast()
            {
                Date = x.Dt,
                TemperatureC = (int)Math.Round(x.Main.Temp),
                Summary = MapFeelToTemp((int)Math.Round(x.Main.Temp))
            })
            // .Skip(1) // drop a current day
            .Take(FORECAST_DAYS)
            .ToList();

        return wfs;

    }
}
