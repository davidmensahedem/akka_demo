using Akka.Actor;
using Demo.Api.Actors.Messages;
using Demo.Api.Services.Interfaces;


namespace Demo.Api.Actors;

public class WebSocketActor : ReceiveActor
{
    private readonly ISocketService _socketService;

    public WebSocketActor(ISocketService socketService)
    {
        _socketService = socketService;

        // Define message handlers
        ReceiveAsync<ConnectWebSocketMessage>(DoHandleConnect);
        ReceiveAsync<SendChannelMessage<string>>(DoSendChannelMessage);
        ReceiveAsync<SendRoomMessage<string>>(DoSendRoomMessage);
        ReceiveAsync<DisconnectWebSocketMessage>(DoHandleDisconnect);
    }

    private async Task DoHandleConnect(ConnectWebSocketMessage message)
    {
        await _socketService.ConnectAsync();
    }

    private async Task DoHandleDisconnect(DisconnectWebSocketMessage message)
    {
        await _socketService.DisconnectAsync();
    }

    private async Task DoSendChannelMessage(SendChannelMessage<string> model) => await _socketService.SendChannelMessageAsync(model.Message);

    private async Task DoSendRoomMessage(SendRoomMessage<string> model) => await _socketService.SendRoomMessageAsync(model.Message);

    protected override void PostStop()
    {
        // Ensure WebSocket is closed when the actor is stopped
        _socketService?.DisconnectAsync();
        base.PostStop();
    }
}
