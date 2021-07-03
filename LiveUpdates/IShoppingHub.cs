using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingListServer.LiveUpdates
{
    public interface IShoppingHub
    {
        public Task SendListAdded(ShoppingList list, ShoppingListPermissionType permission);

        // Send the given list to all users that have the given permission on that list, e.g.
        // if permission == Read then it's send to all users that have read permission on that list.
        public Task SendListUpdated(ShoppingList list, ShoppingListPermissionType permission);

        public Task SendListRemoved(string listSyncId, ShoppingListPermissionType permission);

        public Task SendListRemoved(string listSyncId, string userId);

        // Inform the given user that its permission for the given list changed.
        public Task<bool> SendListPermissionChanged(string listSyncId, string userId, ShoppingListPermissionType permission);

        public Task<bool> SendItemNameChanged(
            string newItemName,
            string oldItemName,
            string listSyncId,
            ShoppingListPermissionType permission);

        public Task<bool> SendItemAddedOrUpdated(
            GenericItem item,
            string listSyncId,
            ShoppingListPermissionType permission);

        public Task<bool> SendProductAddedOrUpdated(
            GenericProduct product,
            string listSyncId,
            ShoppingListPermissionType permission);
    }
}

