namespace Demo.Api.Actors.Messages;

public struct DisconnectWebSocketMessage()
{
    public bool Disconnect { get; set; } = true;
}
