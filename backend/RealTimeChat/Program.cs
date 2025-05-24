using RealTimeChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Удалено: Конфигурация Redis Cache (больше не используется)
// Было:
// builder.Services.AddStackExchangeRedisCache(...);

// Настройка SignalR (оставляем без изменений)
builder.Services.AddSignalR(options => 
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
}).AddJsonProtocol();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://*.railway.app",
                "https://*.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var app = builder.Build();

// Middleware (без изменений)
app.UseRouting();
app.UseCors("CorsPolicy");

// SignalR endpoint (без изменений)
app.MapHub<ChatHub>("/chat", options => 
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

// Health Check (без изменений)
app.MapGet("/", () => "SignalR Server is running. Connect to: ws://localhost:5022/chat");
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));

app.Run();