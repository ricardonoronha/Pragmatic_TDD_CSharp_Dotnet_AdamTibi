using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Uqs.Weather.Services
{

    public record WeatherResponse
    {
        public long Cod;
        public long Message;
        public long Cnt;
        public Forecast[] List;
        public City City;
    }

    public record Forecast
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Dt;
        public Main Main;
        public Weather[] Weather;
        public Clouds Clouds;
        public Wind Wind;
        public long Visibility;
        public double Pop;
        public Rain Rain;
        public Snow Snow;
        public Sys Sys;
        [JsonProperty("dt_txt")]
        public DateTime DtTxt;
    }

    public record Main
    {
        public double Temp;
        [JsonProperty("feels_like")]
        public double FeelsLike;
        [JsonProperty("temp_min")]
        public double TempMin;
        [JsonProperty("temp_max")]
        public double TempMax;
        public long Pressure;
        [JsonProperty("sea_level")]
        public long SeaLevel;
        [JsonProperty("grnd_level")]
        public long GrndLevel;
        public long Humidity;
        [JsonProperty("temp_kf")]
        public double TempKf;
    }

    public record Weather
    {
        public long Id;
        public string Main;
        public string Description;
        public string Icon;
    }

    public record Clouds
    {
        public long All;
    }

    public record Wind
    {
        public double Speed;
        public long Deg;
        public double Gust;
    }

    public record Rain
    {
        [JsonProperty("3h")]
        public decimal ThreeHours;
    }

    public record Snow
    {
        [JsonProperty("3h")]
        public decimal ThreeHours;
    }

    public record Sys
    {
        public string Pod;
    }

    public record City
    {
        public int Id;
        public string Name;
        public Coord Coord;
        public string Country;
        public int Population;
        public long TimeZone;
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Sunrise;
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Sunset;
    }

    public record Coord
    {
        public long Lat;
        public long Lon;
    }
}
