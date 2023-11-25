using Microsoft.AspNetCore.SignalR;

namespace UniTrainingSystem.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(string fromUser,string message)
        {
            await Clients.All.SendAsync("ReceliveMessage",fromUser, message);
        }
    }
}
