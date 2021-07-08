using System;
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
        protected ShoppingHubService _shoppingHubService;

        public UpdateHub_Controller()
        {
            _shoppingHubService = (ShoppingHubService)Startup._serviceProvider.GetRequiredService(typeof(ShoppingHubService));
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
            Console.Beep();
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendListAddedAsync(ShoppingList list, ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendListAdded(list, permission);
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendListUpdated(ShoppingList list, ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendListUpdated(list, permission);
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendListRemoved_Permission(string listSyncId, ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendListRemoved(listSyncId, permission);
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendListRemoved_UserId(string listSyncId, string userId)
        {
            await _shoppingHubService.SendListRemoved(listSyncId, userId);
        }

        // Inform the given user that its permission for the given list changed.
        // [Authorize(Roles = Role.User)]
        public async Task SendListPermissionChanged(string listSyncId, string userId, ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendListPermissionChanged(listSyncId, userId, permission);
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendItemNameChangedAsync(
            string newItemName,
            string oldItemName,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendItemNameChanged(newItemName, oldItemName, listSyncId, permission);
        }
        
        // [Authorize(Roles = Role.User)]
        public async Task SendItemAddedOrUpdatedAsync(
            GenericItem item,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendItemAddedOrUpdated(item, listSyncId, permission);
        }

        // [Authorize(Roles = Role.User)]
        public async Task SendProductAddedOrUpdatedAsync(
            GenericProduct product,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            await _shoppingHubService.SendProductAddedOrUpdated(product, listSyncId, permission);
        }

    }
}
