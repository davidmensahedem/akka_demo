using Akka.Actor;
using Akka.Hosting;
using Demo.Api.Actors;
using Demo.Api.Actors.Messages;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WebSocketController(IRequiredActor<WebSocketActor> webSocketActor) : ControllerBase
{

    [HttpPost("send-channel-message")]
    public async Task<IActionResult> SendChannelMessageAsync([FromBody] string message)
    {
        var channelMessage = new SendChannelMessage<string>("test-channel-id", message);

        webSocketActor.ActorRef.Tell(channelMessage);

        return Ok($"Channel message sent: {message}");
    }

    [HttpPost("send-room-message")]
    public async Task<IActionResult> SendRoomMessage([FromBody] string message)
    {
        var roomMessage = new SendRoomMessage<string>("test-room-id", message);

        webSocketActor.ActorRef.Tell(roomMessage);

        return Ok($"Room message sent: {message}");
    }

    // Endpoint to close the WebSocket connection
    [HttpPost("disconnect")]
    public async Task<IActionResult> Disconnect()
    {
        var disConnectMessage = new DisconnectWebSocketMessage();

        webSocketActor.ActorRef.Tell(disConnectMessage);

        return Ok("WebSocket connection closed.");
    }

    [HttpPost("connect")]
    public async Task<IActionResult> Connect()
    {
        var connect = new ConnectWebSocketMessage();

        webSocketActor.ActorRef.Tell(connect);

        return Ok("WebSocket connection connected again.");
    }
}
