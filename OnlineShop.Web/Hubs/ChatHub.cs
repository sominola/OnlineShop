using Microsoft.AspNetCore.SignalR;

namespace OnlineShop.Web.Hubs;

public class ChatHub : Hub
{
    public string GetConnectionId() =>
        Context.ConnectionId;
}