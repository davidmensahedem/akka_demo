using Demo.Api.Models;

namespace Demo.Api.Actors.Messages
{
    public struct WeatherForecastMessage(WeatherForecast[] weatherForecasts)
    {
        public WeatherForecast[] WeatherForecasts { get; set; } = weatherForecasts;
    }
}
