using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShoppingListServer.Helpers;
using ShoppingListServer.LiveUpdates;
using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;
using ShoppingListServer.Services.Interfaces;

namespace ShoppingListServer.Services
{

    public class ShoppingHubService : IShoppingHub
    {
        private readonly IHubContext<UpdateHub_Controller> _hubContext;
        private readonly AppSettings _appSettings;
        private readonly IShoppingService _shoppingService;

        public ShoppingHubService(IHubContext<UpdateHub_Controller> hubContext, IOptions<AppSettings> appSettings, IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
            _hubContext = hubContext;
            _appSettings = appSettings.Value;
        }

        /*
         * 
         * SINGAL R LIVE UPDATES
         * 
        */
        public async Task SendListAdded(ShoppingList list, ShoppingListPermissionType permission)
        {
            string listJson = JsonConvert.SerializeObject(list);
            List<string> users = _shoppingService.GetUsersWithPermissions(list.SyncId, permission);
            await _hubContext.Clients.Users(users).SendAsync("ListAdded", listJson);
        }

        // Send the given list to all users that have the given permission on that list, e.g.
        // if permission == Read then it's send to all users that have read permission on that list.
        public async Task SendListUpdated(ShoppingList list, ShoppingListPermissionType permission)
        {
            string listJson = JsonConvert.SerializeObject(list);
            List<string> users = _shoppingService.GetUsersWithPermissions(list.SyncId, ShoppingListPermissionType.Read);
            await _hubContext.Clients.Users(users).SendAsync("ListUpdated", listJson);
        }

        public async Task SendListRemoved(string listSyncId, ShoppingListPermissionType permission)
        {
            List<string> users = _shoppingService.GetUsersWithPermissions(listSyncId, ShoppingListPermissionType.Read);
            await _hubContext.Clients.Users(users).SendAsync("ListRemoved", listSyncId);
        }

        public async Task SendListRemoved(string listSyncId, string userId)
        {
            await _hubContext.Clients.Users(userId).SendAsync("ListRemoved", listSyncId);
        }

        // Inform the given user that its permission for the given list changed.
        public async Task<bool> SendListPermissionChanged(
            string listSyncId,
            string userId,
            ShoppingListPermissionType permission)
        {
            try
            {
                await _hubContext.Clients.Users(userId).SendAsync("ListPermissionChanged", listSyncId, permission);
                return true;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("SendListPermissionChanged {0}", ex);
                return false;
            }
        }

        public async Task<bool> SendItemNameChanged(
            string newItemName,
            string oldItemName,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            try
            {
                List<string> users = _shoppingService.GetUsersWithPermissions(listSyncId, permission);
                await _hubContext.Clients.Users(users).SendAsync("ItemNameChanged", listSyncId, newItemName, oldItemName);
                return true;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("SendItemNameChanged {0}", ex);
                return false;
            }
        }

        public async Task<bool> SendItemAddedOrUpdated(
            GenericItem item,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            try
            {
                string itemJson = JsonConvert.SerializeObject(item);
                List<string> users = _shoppingService.GetUsersWithPermissions(listSyncId, permission);
                await _hubContext.Clients.Users(users).SendAsync("ItemAddedOrUpdated", listSyncId, itemJson);
                return true;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("SendItemAddedOrUpdated {0}", ex);
                return false;
            }
        }

        public async Task<bool> SendProductAddedOrUpdated(
            GenericProduct product,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            try
            {
                string productJson = JsonConvert.SerializeObject(product);
                List<string> users = _shoppingService.GetUsersWithPermissions(listSyncId, permission);
                await _hubContext.Clients.Users(users).SendAsync("ProductAddedOrUpdated", listSyncId, productJson);
                return true;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("SendProductAddedOrUpdatedn {0}", ex);
                return false;
            }

        }
    }
}