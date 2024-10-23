namespace SignalRChatApp.WebAPI.Dtos
{
    public class SendMessageDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
