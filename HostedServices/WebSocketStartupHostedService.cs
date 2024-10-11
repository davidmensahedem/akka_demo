
using Akka.Actor;
using Akka.Hosting;
using Demo.Api.Actors;
using Demo.Api.Actors.Messages;

namespace Demo.Api.HostedServices
{
    public class WebSocketStartupHostedService(IRequiredActor<WebSocketActor> webSocketActor) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
            webSocketActor.ActorRef.Tell(new ConnectWebSocketMessage());
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
            webSocketActor.ActorRef.Tell(new DisconnectWebSocketMessage());
        }
    }
}
