using Demo.Api.Services.Interfaces;
using SocketIOClient;

namespace Demo.Api.Services.Providers
{
    public class SocketService : ISocketService
    {
        private readonly SocketIO _socket;
        private const string ChannelId = "your-channel-id-here";
        private const string RoomId = "your-room-id-here";

        public SocketService()
        {
            _socket = new SocketIO("your-socket-server-url");

            var joinChannelRequest = new
            {
                data = new
                {
                    message = "test api message",
                    RoomChannel = ChannelId
                },
                time = DateTime.UtcNow, // Set the current time in UTC
                clientId = _socket.Id, // Replace with the actual client ID
                source = new
                {
                    platform = "web",
                    appName = "TestAPI123",
                    appVersion = "1.0.0",
                    clientId = _socket.Id, // Replace with the actual client ID
                    userName = "TestAPI123",
                    channel = ChannelId
                }
            };

            var roomChannelRequest = new
            {
                data = new
                {
                    roomId = RoomId,
                    message = "test api message",
                    RoomChannel = ChannelId
                },
                time = DateTime.UtcNow, // Set the current time in UTC
                clientId = _socket.Id, // Replace with the actual client ID
                source = new
                {
                    platform = "web",
                    appName = "TestAPI123",
                    appVersion = "1.0.0",
                    clientId = _socket.Id, // Replace with the actual client ID
                    userName = "TestAPI123",
                    channel = ChannelId
                }
            };

            _socket.OnConnected += async (sender, e) =>
            {
                Console.WriteLine("Connected to the Socket.IO server with WebSocket or Polling.");
                await _socket.EmitAsync("join-channel", joinChannelRequest); // Example of emitting an event
                await _socket.EmitAsync("join-room", roomChannelRequest); // Example of emitting an event
            };

            _socket.On("message", async (response) =>
            {
                await Task.Delay(0);
                Console.WriteLine($"Received message: {response}");
            });

            _socket.On(RoomId, async (response) =>
            {
                await Task.Delay(0);
                Console.WriteLine($"Received room message: {response}");
            });

            _socket.On(ChannelId, async (response) =>
            {
                await Task.Delay(0);
                Console.WriteLine($"Received room message: {response}");
            });

            // Handle disconnection
            _socket.OnDisconnected += async (sender, e) =>
            {
                Console.WriteLine("Disconnected from the Socket.IO server ----  started again (:.");
                await RetryConnectionAsync();
            };


        }

        public async Task ConnectAsync()
        {
            try
            {
                await _socket.ConnectAsync();
                Console.WriteLine("Successfully connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        public async Task SendRoomMessageAsync(string message)
        {
            if (_socket.Connected)
            {
                var joinChannelRequest = new
                {
                    data = new
                    {
                        roomId = RoomId,
                        message,
                        RoomChannel = ChannelId
                    },
                    time = DateTime.UtcNow, // Set the current time in UTC
                    clientId = _socket.Id, // Replace with the actual client ID
                    source = new
                    {
                        platform = "web",
                        appName = "TestAPI123",
                        appVersion = "1.0.0",
                        clientId = _socket.Id, // Replace with the actual client ID
                        userName = "TestAPI123",
                        channel = ChannelId
                    }
                };

                await _socket.EmitAsync("notify-room", message);
                Console.WriteLine("Room Message sent: " + message);
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }
        }

        public async Task SendChannelMessageAsync(string message)
        {
            if (_socket.Connected)
            {
                var joinChannelRequest = new
                {
                    data = new
                    {
                        message,
                        RoomChannel = ChannelId
                    },
                    time = DateTime.UtcNow, // Set the current time in UTC
                    clientId = _socket.Id, // Replace with the actual client ID
                    source = new
                    {
                        platform = "web",
                        appName = "TestAPI123",
                        appVersion = "1.0.0",
                        clientId = _socket.Id, // Replace with the actual client ID
                        userName = "TestAPI123",
                        channel = ChannelId
                    }
                };

                await _socket.EmitAsync(ChannelId, message);
                Console.WriteLine("Channel Message sent: " + message);
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }
        }

        public async Task DisconnectAsync()
        {
            await _socket.DisconnectAsync();
            Console.WriteLine("Socket connection disconnected");
        }

        private async Task RetryConnectionAsync()
        {
            int retryCount = 0;
            int maxRetries = 10;
            int delay = 3000; // 2 seconds

            await _socket.DisconnectAsync();

            while (retryCount < maxRetries && (_socket == null || !_socket.Connected))
            {
                retryCount++;

                try
                {
                    await _socket!.ConnectAsync();
                    if (_socket.Connected)
                    {
                        break; // Exit if connection is successful
                    }
                }
                catch (Exception ex)
                {
                    await RetryConnectionAsync();
                }

                await Task.Delay(delay);
            }

            if (!_socket!.Connected)
            {
                await RetryConnectionAsync();
            }
        }
    }

    public class JoinChannelRequest
    {
        public ChannelData Data { get; set; }
        public DateTime Time { get; set; }
        public string ClientId { get; set; }
        public SourceInfo Source { get; set; }
    }

    public class ChannelData
    {
        public string Message { get; set; }
        public string RoomId { get; set; }  // Nullable
        public string RoomChannel { get; set; }
    }

    public class SourceInfo
    {
        public string Platform { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Channel { get; set; }
    }
}


