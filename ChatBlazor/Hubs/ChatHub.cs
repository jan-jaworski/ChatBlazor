using ChatBlazor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChatBlazor.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

      

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

       
    }
}
