﻿using ShoppingListServer.Entities;
using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingListServer.LiveUpdates
{
    public interface IShoppingHub
    {
        Task SendListAdded(User user, ShoppingList list, ShoppingListPermissionType permission);

        // Send the given list to all users that have the given permission on that list, e.g.
        // if permission == Read then it's send to all users that have read permission on that list.
        Task SendListUpdated(User user, ShoppingList list, ShoppingListPermissionType permission);

        Task SendListRemoved(User user, string listSyncId, ShoppingListPermissionType permission);

        Task SendListRemoved(User user, string listSyncId, string userId);

        // Inform the given user that its permission for the given list changed.
        Task<bool> SendListPermissionChanged(
            User thisUser,
            string listSyncId,
            string targetUserId,
            ShoppingListPermissionType permission);

        Task<bool> SendItemNameChanged(
            User user,
            string newItemName,
            string oldItemName,
            string listSyncId,
            ShoppingListPermissionType permission);

        Task<bool> SendItemAddedOrUpdated(
            User user,
            GenericItem item,
            string listSyncId,
            ShoppingListPermissionType permission);

        Task<bool> SendProductAddedOrUpdated(
            User user,
            GenericProduct product,
            string listSyncId,
            ShoppingListPermissionType permission);
    }
}

