using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Models;

namespace RealTimeChat.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string userName, string message);
}

public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation($"New connection: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnConnectedAsync error");
            throw;
        }
    }

    public async Task JoinChat(UserConnection connection)
    {
        try 
        {
            // Сохраняем данные подключения в памяти
            _connections.TryAdd(Context.ConnectionId, connection);

            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
            await Clients.Group(connection.ChatRoom)
                .ReceiveMessage("System", $"{connection.UserName} присоединился к чату");
            
            _logger.LogInformation($"User {connection.UserName} joined {connection.ChatRoom}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JoinChat error");
            throw new HubException($"JoinChat failed: {ex.Message}");
        }
    }

    public async Task SendMessage(string message)
    {
        try
        {
            var connection = GetUserConnection();
            if (connection == null) return;

            await Clients
                .Group(connection.ChatRoom)
                .ReceiveMessage(connection.UserName, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendMessage error");
            throw new HubException($"SendMessage failed: {ex.Message}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            if (_connections.TryRemove(Context.ConnectionId, out var connection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);
                
                await Clients
                    .Group(connection.ChatRoom)
                    .ReceiveMessage("System", $"{connection.UserName} покинул чат");
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnDisconnectedAsync error");
            throw;
        }
    }

    private UserConnection? GetUserConnection()
    {
        try
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var connection))
            {
                return connection;
            }

            _logger.LogWarning($"No connection data for {Context.ConnectionId}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserConnection error");
            return null;
        }
    }
}