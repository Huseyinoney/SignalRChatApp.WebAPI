using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Models;
using System.Collections;

namespace SignalRChatApp.WebAPI.Hubs
{
    public class ChatHub(AppDbContext dbContext) : Hub
    {
        public static List<User> UsersList = [];
        //private readonly TimeSpan _disconnectTimeout = TimeSpan.FromMilliseconds(10);

        public async Task Connect(string userId)
        {
           Guid userGuidId =  Guid.Parse(userId);
            Console.WriteLine(userGuidId);
            User user = await dbContext.Users.FindAsync(userGuidId);

            if (user is not null)
            {
                // Kullanıcının zaten listede olup olmadığını kontrol edin
                var existingUser = UsersList.FirstOrDefault(u => u.Id == userGuidId);

                if (existingUser is not null)
                {
                    // Kullanıcı zaten varsa sadece ConnectionId'yi güncelleyin
                    existingUser.ConnectionId = Context.ConnectionId;
                    existingUser.Status = "online";
                    existingUser.LastActivity = DateTime.UtcNow;
                }
                else
                {
                    // Kullanıcı yeni ise listeye ekleyin
                    user.ConnectionId = Context.ConnectionId;
                    user.Status = "online";
                    user.LastActivity = DateTime.UtcNow;
                    UsersList.Add(user);
                }
               
                await dbContext.SaveChangesAsync();
                await Clients.All.SendAsync("Users", UsersList);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("ondisconnected çalıştı");
            Guid userId;
            Console.WriteLine(Context.ConnectionId);
            //connectionId fazlalığı var ona bak
            var userOnUserList = UsersList.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if(userOnUserList != null)
            {
                 userId = userOnUserList.Id;
            }
            else
            {
                return;
            }
            //Console.WriteLine(userId);
            User user = await dbContext.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                
                await dbContext.SaveChangesAsync();
                //UsersList.Remove(user);
                await Clients.All.SendAsync("Users", UsersList);
                
            }
        }

        public async Task SendMessage( string message)
        {
            await Clients.All.SendAsync("ReceivedMessage", message);
        }

        public async Task SetUserOffline(string connectionId)
        {
            var userOnUserList = UsersList.FirstOrDefault(u => u.ConnectionId == connectionId);
            if (userOnUserList is not null)
            {
                userOnUserList.Status = "offline";
                await Clients.All.SendAsync("Users", UsersList); // Tüm istemcilere yeni kullanıcı listesi gönderilir
               // Console.WriteLine(UsersList);
                // Veritabanında da durumu güncelleyin
                var user = await dbContext.Users.FindAsync(userOnUserList.Id);
                if (user is not null)
                {
                    
                    user.Status = "offline";
                    UsersList.Remove(user);
                    await dbContext.SaveChangesAsync();
                }
            }
        }



    }
}
