using Microsoft.AspNetCore.Http;
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
    public class AuthController(AppDbContext dbContext, IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (registerDto.Name is null || registerDto.Name.Equals("")) { return StatusCode(403, new { Message = "Kullanıcı Adı Boş Olamaz" }); }

            bool isNameExists = await dbContext.Users.AnyAsync(p => p.Name == registerDto.Name);
            if (isNameExists)
            {
                return BadRequest(new { Message = "Kullanıcı Adı Daha Önce Kullanılmış" });
            }
            User user = new()
            {
                Name = registerDto.Name
            };
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return StatusCode(201, new
            {
                Message = "Kayıt İşlemi Başarılı"
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto.Name is null || loginDto.Name.Equals("")) { return StatusCode(403, new { Message = "Kullanıcı Adı Boş Olamaz" }); }

            User? user = await dbContext.Users.FirstOrDefaultAsync(p => p.Name == loginDto.Name);

            if (user is null)
            {
                return BadRequest(new { Message = "Kullanıcı Bulunamadı" });
            }
            user.Status = "online";
            await dbContext.SaveChangesAsync();

            return StatusCode(200, new
            {
                user,
                Message = "Giriş İşlemi Başarılı"
            });
        }
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut(LogOutDto logOutDto)
        {

            var userOnUserList = ChatHub.UsersList.FirstOrDefault(n => n.Name == logOutDto.Name);
            if (userOnUserList is not null)
            {
                userOnUserList.Status = "offline";
                await hubContext.Clients.All.SendAsync("Users", ChatHub.UsersList); // Tüm istemcilere yeni kullanıcı listesi gönderilir
                Console.WriteLine(ChatHub.UsersList);
                // Veritabanında da durumu güncelleyin
                var user = await dbContext.Users.FindAsync(userOnUserList.Id);
                if (user is not null)
                {
                    user.Status = "offline";
                    
                    await dbContext.SaveChangesAsync();
                }
                return StatusCode(200, new { Message = "Kullanıcı Çıkış Yaptı" });
            }
            return BadRequest(new { Message = "İşlem Başarısız" });
        }
    }
}
