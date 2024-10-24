﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Dtos;
using SignalRChatApp.WebAPI.Hubs;
using SignalRChatApp.WebAPI.Models;

namespace SignalRChatApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController(AppDbContext dbContext,
        IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpPost("AllChats")]
        public async Task<IActionResult> GetChats(Guid userId, Guid toUserId)
        {
            List<Chat> chats = await dbContext.Chats
                .Where(p => p.UserId == userId && p.ToUserId == toUserId ||
                p.ToUserId == userId && p.UserId == toUserId)
                .OrderBy(p => p.Date).ToListAsync();
            return Ok(chats);
        }
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto)
        {

            Chat chat = new()
            {
                UserId = Guid.Parse(sendMessageDto.UserId),
                ToUserId = Guid.Parse(sendMessageDto.ToUserId),
                Message = sendMessageDto.Message,
            };
            await dbContext.AddAsync(chat);
            await dbContext.SaveChangesAsync();
            string connectionId = ChatHub.UsersList.First(p => p.Id == chat.ToUserId).ConnectionId;
            await hubContext.Clients.Client(connectionId).SendAsync("SendMessageToUser", chat);
            return Ok();
        }

    }
}
