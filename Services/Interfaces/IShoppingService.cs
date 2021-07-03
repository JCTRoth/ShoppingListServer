﻿using ShoppingListServer.Models;
using ShoppingListServer.Models.Commands;
using ShoppingListServer.Models.ShoppingData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListServer.Services.Interfaces
{
        public interface IShoppingService
        {
            string GetID();
            ShoppingList GetList(string userId, string shoppingListId);
            List<ShoppingListWithPermissionDTO> GetLists(string userId);
            // Return all lists that the user has the given permission for.
            List<ShoppingList> GetLists(string userId, ShoppingListPermissionType permission);
            Task<bool> AddList(ShoppingList list, string userID);
            Task<bool> UpdateList(ShoppingList list, string userId);
            Task<bool> DeleteList(string shoppingListId, string userId);
            Task<bool> Update_Item_In_List(string itemNameOld, GenericItem itemNew, string userId, string shoppingListId);
            Task<bool> Add_Or_Update_Product_In_List(GenericProduct productNew, string userId, string shoppingListId);
            Task<bool> Remove_Item_In_List(string itemName, string userId, string shoppingListId);

            // Returns all permissions that are assigned to a list.
            // \return List<Tuple<UserId, ShoppingListPermissionType>>
            List<Tuple<string, ShoppingListPermissionType>> GetListPermissions(string shoppingListId);

            // Returns all list permissions of a certain user. These are all lists that a user can at least read.
            // \return List<Tuple<ShoppingListId, ShoppingListPermissionType>>
            List<Tuple<string, ShoppingListPermissionType>> GetUserListPermissions(string userId);

            // Return permission that the given user has for the given list
            // Exception: If this user has no read access to the list.
            ShoppingListPermissionType GetUserListPermission(string shoppingListId, string thisUserId, string userId);

            Task<bool> AddOrUpdateListPermission(string thisUserId, string targetUserId, string shoppingListId, ShoppingListPermissionType permission);
            // Remove the permission of a user from a shopping list. Doesn't work if the user is the owner.

            Task<bool> RemoveListPermission(string thisUserId, string targetUserId, string shoppingListId);

            List<string> GetUsersWithPermissions(string listSyncId, ShoppingListPermissionType permission);

        }
}
