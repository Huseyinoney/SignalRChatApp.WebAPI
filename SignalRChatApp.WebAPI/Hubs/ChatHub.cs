using Microsoft.AspNetCore.SignalR;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Models;

namespace SignalRChatApp.WebAPI.Hubs
{
    public class ChatHub(AppDbContext dbContext) : Hub
    {
        public static Dictionary<string, Guid> Users = new();

        public async Task Connect(Guid userId)
        {
            Users.Add(Context.ConnectionId, userId);
            User? user = await dbContext.Users.FindAsync(userId);

            if (user is not null)
            {
                user.Status = "online";
                await dbContext.SaveChangesAsync();
                await Clients.All.SendAsync("Users", user);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Guid userId;
            Users.TryGetValue(Context.ConnectionId, out userId);
            User? user = await dbContext.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                await dbContext.SaveChangesAsync();
                await Clients.All.SendAsync("Users", user);
            }
        }

    }
}
