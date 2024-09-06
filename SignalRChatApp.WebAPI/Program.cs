using Microsoft.EntityFrameworkCore;
using SignalRChatApp.WebAPI.Context;
using SignalRChatApp.WebAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServer")
    ));
builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
policy.AllowAnyMethod()
.AllowCredentials()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.Run();
