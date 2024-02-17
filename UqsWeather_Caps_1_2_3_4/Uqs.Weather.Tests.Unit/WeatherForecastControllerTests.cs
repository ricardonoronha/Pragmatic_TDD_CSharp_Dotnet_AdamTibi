using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.ReceivedExtensions;
using Uqs.Weather.Controllers;
using Uqs.Weather.Services;

namespace Uqs.Weather.Testes.Unit;

public class WeatherForecastControllerTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(-100, -148)]
    [InlineData(-10.1, 13.82)]
    [InlineData(10, 50)]
    [InlineData(1, 33.8)]
    [InlineData(-1, 30.2)]
    public void ConvertCToF_Celsius_Fahrenheit(double c, double f)
    {
        var logger = NullLogger<WeatherForecastController>.Instance;
        var controller = new WeatherForecastController(null!, logger, null!, null!, null!);
        double atual = controller.ConvertCToF(c);
        Assert.Equal(f, atual);

    }


    [Fact]
    public async void Test_ClientCall()
    {
        // Arrange

        var today = DateTime.Now.Date;
        var weathersTemp = new[] { 1d, 2d, 3d, 4d, 5d, 6d, 7d };

        var clientMock = Substitute.For<IClient>();

        clientMock
            .WeatherCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<Units>())
            .Returns(x =>
            {
                const int DAYS = 7;
                var res = new WeatherResponse();
                res.List = new Forecast[DAYS];
                for (int i = 0; i < DAYS; i++)
                {
                    var item = new Forecast();
                    item.Main = new Main();
                    item.Dt = today.AddDays(i);
                    item.Main.Temp = weathersTemp[i];
                    res.List[i] = item;
                }
                return Task.FromResult(res);
            });

        var logger = NullLogger<WeatherForecastController>.Instance;

        var controller = new WeatherForecastController(null!, logger, null!, null!, clientMock);

        // Act

        await controller.GetReal();

        // Assert

        await clientMock
            .Received()
            .WeatherCallAsync(Arg.Is<decimal>(x => x == -5.53394m), Arg.Is<decimal>(x => x == -40.7751m), Arg.Is<Units>(x => x == Units.Metric));
    }
}