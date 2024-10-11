namespace Demo.Api.Actors.Messages
{
    public struct SendChannelMessage<T>(string channelId, T message)
    {
        public string ChannelId { get; set; } = channelId;
        public T Message { get; set; } = message;
    }
}
