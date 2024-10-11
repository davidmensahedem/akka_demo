namespace Demo.Api.Services.Interfaces
{
    public interface ISocketService
    {
        Task SendRoomMessageAsync(string message);
        Task ConnectAsync();
        Task DisconnectAsync();
        Task SendChannelMessageAsync(string message);
    }
}