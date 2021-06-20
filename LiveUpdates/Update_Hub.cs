using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShoppingListServer.Controllers
{
    [Authorize]
        public class Update_Hub : Hub
        {
            public override async Task OnConnectedAsync()
            {
                await Clients.Caller.SendAsync("ServerMessage", $"Connected to {Context.UserIdentifier}");
                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception exception)
            {
                await Clients.Caller.SendAsync("ServerMessage", $"Disconnected {Context.UserIdentifier}");
                await base.OnDisconnectedAsync(exception);
            }

            public async Task SendToUser(string user, string message)
            {
                await Clients.User(user).SendAsync("ServerMessage", $"{Context.UserIdentifier}: {message}");
            }


        }
}
