namespace SignalRChatApp.WebAPI.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public string ConnectionId { get; set; } =string.Empty;

         public DateTime LastActivity {  get; set; }
    }
}
