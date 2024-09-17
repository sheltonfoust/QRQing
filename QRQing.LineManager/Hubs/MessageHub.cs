using Microsoft.AspNetCore.SignalR;

namespace QRQing.LineManager.Hubs;

public class MessageHub : Hub
{
    public async Task SendUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveUpdate", message);
    }

    
}
