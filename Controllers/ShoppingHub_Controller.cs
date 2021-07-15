﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ShoppingListServer.Entities;
using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;
using ShoppingListServer.Services;

namespace ShoppingListServer.LiveUpdates
{
    [Authorize]
    public class UpdateHub_Controller : Hub
    {
        protected IShoppingHub _shoppingHubService;

        public UpdateHub_Controller(IShoppingHub shoppingHubService)
        {
            _shoppingHubService = shoppingHubService;
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
            Console.Error.WriteLine("OnDisconnectAsync {0}", exception);
        }
    }
}
