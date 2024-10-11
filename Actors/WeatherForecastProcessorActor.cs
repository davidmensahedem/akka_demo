using Akka.Actor;
using Demo.Api.Actors.Messages;

namespace Demo.Api.Actors
{
    public class WeatherForecastProcessorActor : ReceiveActor
    {
        private readonly ILogger<WeatherForecastProcessorActor> _logger;

        public WeatherForecastProcessorActor(ILogger<WeatherForecastProcessorActor> logger)
        {
            _logger = logger;

            ReceiveAsync<WeatherForecastMessage>(ProcessWeatherForecast);
        }

        private async Task ProcessWeatherForecast(WeatherForecastMessage message)
        {
            try
            {
                var weatherForecast = message.WeatherForecasts.OrderByDescending(x => x.TemperatureC).First();

                _logger.LogDebug("The highest temperature of {Temperature} degrees Celsius was recorded in {Country}",
                    weatherForecast.TemperatureC, weatherForecast.Country);

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while processing the weather forecast data");
            }
        }
    }
}
