using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Dtos;
using SignalRChatApp.WebAPI.Models;

namespace SignalRChatApp.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(AppDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            bool isNameExists = await dbContext.Users.AnyAsync(p => p.Name == registerDto.Name);
            if (isNameExists)
            {
                return BadRequest(new {Message = "Kullanıcı Adı Daha Önce Kullanılmış"});
            }
            User user = new()
            {
                Name = registerDto.Name
            };
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            User? user = await dbContext.Users.FirstOrDefaultAsync(p => p.Name == loginDto.Name);
            if (user is null) 
            {
                return BadRequest(new { Message = "Kullanıcı Bulunamadı" });
            }
            user.Status = "online";
            await dbContext.SaveChangesAsync();

            return Ok(user);
        }




    }
}
