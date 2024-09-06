using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Dtos;
using SignalRChatApp.WebAPI.Hubs;
using SignalRChatApp.WebAPI.Models;
using System.Reflection.Metadata.Ecma335;

namespace SignalRChatApp.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatsController(AppDbContext dbContext,
        IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetChats(Guid userId, Guid toUserId)
        {
            List<Chat> chats = await dbContext.Chats
                .Where(p => p.UserId == userId && p.ToUserId == toUserId ||
                p.ToUserId == userId && p.UserId == toUserId)
                .OrderBy(p => p.Date).ToListAsync();
            return Ok(chats);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto)
        {
            Chat chat = new()
            {
                UserId = sendMessageDto.UserId,
                ToUserId = sendMessageDto.ToUserId,
                Message = sendMessageDto.Message,
                Date = sendMessageDto.Date,
            };
            await dbContext.AddAsync(chat);
            await dbContext.SaveChangesAsync();
            string connectionId = ChatHub.Users.First(p => p.Value == chat.ToUserId).Key;
            await hubContext.Clients.Client(connectionId).SendAsync("Messages", chat);
            return Ok();
        }

    }
}
