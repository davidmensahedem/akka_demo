namespace Demo.Api.Actors.Messages
{
    public struct SendRoomMessage<T>(string roomId, T message)
    {
        public string RoomId { get; set; } = roomId;
        public T Message { get; set; } = message;
    }
}
