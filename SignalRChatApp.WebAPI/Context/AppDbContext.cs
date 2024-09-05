using Microsoft.EntityFrameworkCore;
using SignalRChatApp.WebAPI.Models;

namespace SignalRChatApp.WebAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}
