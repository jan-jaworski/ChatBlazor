using ChatBlazor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChatBlazor.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        

        private static Dictionary<string,string> connectedUsers = new Dictionary<string, string>();
        public static IReadOnlyDictionary<string, string> ConnectedUsers => connectedUsers;


        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                connectedUsers.Add(userId,Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }





        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                connectedUsers.Remove(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }

    }
}
