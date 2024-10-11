using Akka.Actor;
using Akka.Hosting;
using Demo.Api.Actors;
using Demo.Api.Actors.Messages;
using Demo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace Demo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IActorRegistry registry) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "India",
            "Ghana",
            "Togo",
            "Benin",
            "Nigeria",
            "Spain",
            "Chad",
            "Qatar",
            "Malta"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var results = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Country = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            registry.Get<WeatherForecastProcessorActor>().Tell(new WeatherForecastMessage { WeatherForecasts = results });

            return results;
        }

        [HttpGet("LocationData")]
        public string LocationData()
        {
            return "say";
        }


        [HttpPost("connect")]
        public async Task<IActionResult> ConnectWebSocket()
        {
            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                try
                {
                    var _serverUri = new Uri("ws://localhost:5000/ws");
                    // Connect to the WebSocket server
                    await webSocket.ConnectAsync(_serverUri, CancellationToken.None);
                    Console.WriteLine("Connected to the WebSocket server.");

                    // Send an initial message (Optional)
                    var message = "Hello, server!";
                    var bytesToSend = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Receive message from the server
                    var buffer = new byte[1024 * 4];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!result.CloseStatus.HasValue)
                    {
                        var serverMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Message from server: {serverMessage}");

                        // Optionally, you can send more messages or just listen
                        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }

                    // Close the WebSocket connection
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    return Ok("Connection closed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket Error: {ex.Message}");
                    return StatusCode(500, "Failed to connect to WebSocket server");
                }
            }
        }
    }
}


