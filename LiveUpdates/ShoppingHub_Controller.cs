using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShoppingListServer.Services;

namespace ShoppingListServer.LiveUpdates
{
    [Authorize]
        public class Update_Hub : Hub
        {
            protected ShoppingService _shoppingService;
            
            public Update_Hub(ShoppingService shoppingService)
            {
                _shoppingService = shoppingService;
            }
            
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
    }
}
