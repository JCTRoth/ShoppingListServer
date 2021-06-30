using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShoppingListServer.Database;
using ShoppingListServer.Entities;
using ShoppingListServer.Exceptions;
using ShoppingListServer.Helpers;
using ShoppingListServer.LiveUpdates;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;
using ShoppingListServer.Models.Commands;
using ShoppingListServer.Models.ShoppingData;

namespace ShoppingListServer.Services
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

    }

    public class ShoppingService : IShoppingService
    {
        private readonly IHubContext<Update_Hub> _hubContext;
        private readonly AppSettings _appSettings;
        private readonly Json_Files _json_files;
        private readonly AppDb _db;

        public ShoppingService(IHubContext<Update_Hub> hubContext, IOptions<AppSettings> appSettings, AppDb db)
        {
            _hubContext = hubContext;
            _appSettings = appSettings.Value;
            _json_files = new Json_Files();
            _db = db;
        }

        public string GetID()
        {
            string new_id = Guid.NewGuid().ToString();
            return new_id;
        }

        public ShoppingList GetList(string userId, string shoppingListId)
        {
            ShoppingList list = GetShoppingListEntity(shoppingListId);
            if (list == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(list, userId, ShoppingListPermissionType.Read);
            return LoadShoppingList(shoppingListId);
        }

        public List<ShoppingList> GetLists(string userId, ShoppingListPermissionType permission)
        {
            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(userId) && perm.PermissionType.HasFlag(permission)
                        select list;

            List<ShoppingList> listEntities = query.ToList();
            List<ShoppingList> lists = new List<ShoppingList>();
            foreach (ShoppingList listEntity in listEntities)
            {
                ShoppingList list = LoadShoppingList(listEntity.SyncId);
                if (list != null)
                    lists.Add(list);
            }
            return lists;
        }

        public List<ShoppingListWithPermissionDTO> GetLists(string userId)
        {
            // How to get tuple out of querry: https://stackoverflow.com/a/33545601
            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(userId)
                        select new ShoppingListWithPermissionDTO( list, perm.UserId, perm.PermissionType );
            return query.ToList();
            //return _db.ShoppingLists.Where(list => CheckPermission(list, userId, permission)).ToList();
        }

        // Adds the given list to this server.
        // Sets list.Id and list.ShoppingListPermissions
        public async Task<bool> AddList(ShoppingList list, string userID)
        {
            ShoppingList existingList = GetShoppingListEntity(list.SyncId);
            if (existingList != null)
            {
                return false;
            }
            else
            {
                list.SyncId = Guid.NewGuid().ToString();
                list.ShoppingListPermissions = new List<ShoppingListPermission>();
                list.ShoppingListPermissions.Add(new ShoppingListPermission()
                {
                    PermissionType = ShoppingListPermissionType.All,
                    ShoppingList = list,
                    UserId = userID
                });
                
                if (_json_files.Store_ShoppingList(userID, list))
                {
                    _db.ShoppingLists.Add(list);
                    _db.SaveChanges();
                    await SendListAdded(list, ShoppingListPermissionType.Read);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Overwrites the given shopping list with the stored one that has the same Id.
        // Throws ShoppingListNotFoundException if there is no such list stored. Use AddList in that case first.
        public async Task<bool> UpdateList(ShoppingList list, string userId)
        {
            ShoppingList listEntity = GetShoppingListEntity(list.SyncId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(list.SyncId);
            CheckPermissionWithException(listEntity, userId, ShoppingListPermissionType.Write);
            return await UpdateShoppingList(list);
        }

        public async Task<bool> DeleteList(string shoppingListId, string userId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(listEntity, userId, ShoppingListPermissionType.Delete);
            _db.ShoppingLists.Remove(listEntity);
            _db.SaveChanges();
            bool success = DeleteShoppingList(shoppingListId);
            if (success)
            {
                await SendListRemoved(shoppingListId, ShoppingListPermissionType.Read);
            }
            return success;
        }

        // Updates itemNameOld with itemNew.
        // If there is no itemNameOld, does noting.
        public async Task<bool> Update_Item_In_List(string itemNameOld, GenericItem itemNew, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = LoadShoppingList(shoppingListId);

            bool success = false;
            if (list != null)
            {
                int index = list.ProductList.FindIndex(prod => prod.Item.Name == itemNameOld);
                if (index != -1)
                {
                    list.ProductList[index].Item = itemNew;
                    success = await UpdateShoppingList(list);
                    if (success)
                    {
                        await SendItemAddedOrUpdated(itemNew, list.SyncId, ShoppingListPermissionType.Read);
                    }
                }
            }
            return success;
        }

        // Updates the given product. Only product information should change, nothing from the item, like the name.
        // If the product is not part of the list, it is added.
        public async Task<bool> Add_Or_Update_Product_In_List(GenericProduct productNew, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = LoadShoppingList(shoppingListId);

            if (list != null)
            {
                int index = list.ProductList.FindIndex(prod => prod.Item.Name == productNew.Item.Name);
                if (index != -1)
                {
                    list.ProductList[index] = productNew;
                }
                else
                {
                    list.ProductList.Add(productNew);
                }
                bool success = await UpdateShoppingList(list);
                if (success)
                {
                    await SendProductAddedOrUpdated(productNew, list.SyncId, ShoppingListPermissionType.Read);
                }
            }
            return false;
        }

        public async Task<bool> Remove_Item_In_List(string itemName, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = LoadShoppingList(shoppingListId);

            if (list != null)
            {
                int index = list.ProductList.FindIndex(prod => prod.Item.Name == itemName);
                if (index != -1)
                {
                    list.ProductList.RemoveAt(index);
                    bool success = await UpdateShoppingList(list);
                    if (success)
                    {
                        await SendListUpdated(list, ShoppingListPermissionType.Read);
                    }
                }
            }
            return false;
        }

        // Return List<Tuple<UserId, Permission>>
        public List<Tuple<string, ShoppingListPermissionType>> GetListPermissions(string shoppingListId)
        {
            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.ShoppingListId.Equals(shoppingListId)
                        select new Tuple<string, ShoppingListPermissionType>(perm.UserId, perm.PermissionType);
            return query.ToList();
        }

        // Return List<Tuple<ShoppingListId, Permission>>
        public List<Tuple<string, ShoppingListPermissionType>> GetUserListPermissions(string userId)
        {
            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(userId)
                        select new Tuple<string, ShoppingListPermissionType>(list.SyncId, perm.PermissionType);
            return query.ToList();
        }

        public ShoppingListPermissionType GetUserListPermission(string shoppingListId, string thisUserId, string targetUserId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(listEntity, thisUserId, ShoppingListPermissionType.Read);

            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(targetUserId) && perm.ShoppingListId.Equals(shoppingListId)
                        select perm.PermissionType;
            return query.ToList().FirstOrDefault();
        }

        private List<string> GetUsersWithPermissions(string listSyncId, ShoppingListPermissionType permission)
        {
            List<Tuple<string, ShoppingListPermissionType>> tuples = GetListPermissions(listSyncId);
            List<string> users = new List<string>();
            foreach (Tuple<string, ShoppingListPermissionType> tuple in tuples)
            {
                if (tuple.Item2.HasFlag(permission))
                {
                    users.Add(tuple.Item1);
                }
            }
            return users;
        }

        // \param thisUser - the user who tries to change the permission
        // \param targetUser - the user whose permission should be changed
        // \param shoppingListId - id of the list whose permission is changed
        // \param permission - target permission type
        public async Task<bool> AddOrUpdateListPermission(string thisUserId, string targetUserId, string shoppingListId, ShoppingListPermissionType permission)
        {
            ShoppingList targetList = GetShoppingListEntity(shoppingListId);
            if (targetList == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(targetList, thisUserId, ShoppingListPermissionType.ModifyAccess);

            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(targetUserId) && perm.ShoppingListId.Equals(shoppingListId)
                        select new { list, perm };
            var first = query.FirstOrDefault();
            if (first != null)
            {
                first.perm.PermissionType = permission;
            }
            else
            {
                ShoppingList list = GetShoppingListEntity(shoppingListId);
                if (list == null)
                    throw new ShoppingListNotFoundException(shoppingListId);
                list.ShoppingListPermissions.Add(new ShoppingListPermission
                {
                    ShoppingListId = shoppingListId,
                    UserId = targetUserId,
                    PermissionType = permission
                });
            }
            _db.SaveChanges();
            await SendListPermissionChanged(shoppingListId, targetUserId, permission);
            return true;
        }

        public async Task<bool> RemoveListPermission(string thisUserId, string targetUserId, string shoppingListId)
        {
            ShoppingList targetList = GetShoppingListEntity(shoppingListId);
            if (targetList == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermissionWithException(targetList, thisUserId, ShoppingListPermissionType.ModifyAccess);

            var query = from list in _db.Set<ShoppingList>()
                        join perm in _db.Set<ShoppingListPermission>()
                            on list.SyncId equals perm.ShoppingListId
                        where perm.UserId.Equals(targetUserId) && perm.ShoppingListId.Equals(shoppingListId)
                        select new { list, perm };
            var first = query.FirstOrDefault();
            if (first != null)
            {
                first.list.ShoppingListPermissions.Remove(first.perm);
                _db.SaveChanges();

                // Remove the list for the user whose permission was removed.
                await SendListRemoved(shoppingListId, targetUserId);
                return true;
            }
            return false;
        }

        // Returns the entity of the given shopping list. This is an object that has only the fields set that are in the database.
        // Fields that are marked with [NotMapped] will be null. This is only the "hull" of a list that should be used for query
        // operations on the database or to fetch the missing information from json files.
        private ShoppingList GetShoppingListEntity(string shoppingListId)
        {
            return _db.ShoppingLists.FirstOrDefault(ShoppingList => ShoppingList.SyncId == shoppingListId);
        }

        private ShoppingListPermission GetPermission(ShoppingList list, string userId)
        {
            var permissions = list.ShoppingListPermissions.Where(per => per.UserId == userId).ToList();
            return permissions.FirstOrDefault();
        }

        private bool CheckPermission(ShoppingListPermission permission, ShoppingListPermissionType expectedPermission)
        {
            return permission != null && permission.PermissionType.HasFlag(expectedPermission);
        }

        private bool CheckPermission(ShoppingList list, string userId, ShoppingListPermissionType expectedPermission)
        {
            return CheckPermission(GetPermission(list, userId), expectedPermission);
        }

        private void CheckPermissionWithException(ShoppingList list, string userId, ShoppingListPermissionType expectedPermission)
        {
            ShoppingListPermission permission = GetPermission(list, userId);
            if (!CheckPermission(permission, expectedPermission))
                throw new NoShoppingListPermissionException(permission, expectedPermission);
        }

        private string GetOwnerId(string shoppingListId)
        {
            var query = from perm in _db.Set<ShoppingListPermission>()
                        where perm.ShoppingListId == shoppingListId && perm.PermissionType.HasFlag(ShoppingListPermissionType.All)
                        select perm.UserId;
            var owner = query.FirstOrDefault();
            if (owner != null)
            {
                return owner;
            }
            return null;
        }

        // Loads the json file of the shopping list with the given id.
        private ShoppingList LoadShoppingList(string shoppingListId)
        {
            string ownerId = GetOwnerId(shoppingListId);
            if (ownerId != null)
            {
                return _json_files.Load_ShoppingList(ownerId, shoppingListId);
            }
            return null;
        }

        private bool DeleteShoppingList(string shoppingListId)
        {
            string ownerId = GetOwnerId(shoppingListId);
            if (ownerId != null)
            {
                return _json_files.Delete_ShoppingList(ownerId, shoppingListId);
            }
            return false;
        }

        private async Task<bool> UpdateShoppingList(ShoppingList list)
        {
            string ownerId = GetOwnerId(list.SyncId);
            if (ownerId != null)
            {
                bool success = _json_files.Store_ShoppingList(ownerId, list);
                if (success)
                {
                    await SendListUpdated(list, ShoppingListPermissionType.Read);
                }
                return success;
            }
            return false;
        }
        
        private async Task SendListAdded(ShoppingList list, ShoppingListPermissionType permission)
        {
            string listJson = JsonConvert.SerializeObject(list);
            List<string> users = GetUsersWithPermissions(list.SyncId, permission);
            await _hubContext.Clients.Users(users).SendAsync("ListAdded", listJson);
        }

        // Send the given list to all users that have the given permission on that list, e.g.
        // if permission == Read then it's send to all users that have read permission on that list.
        private async Task SendListUpdated(ShoppingList list, ShoppingListPermissionType permission)
        {
            string listJson = JsonConvert.SerializeObject(list);
            List<string> users = GetUsersWithPermissions(list.SyncId, ShoppingListPermissionType.Read);
            await _hubContext.Clients.Users(users).SendAsync("ListUpdated", listJson);
        }

        private async Task SendListRemoved(string listSyncId, ShoppingListPermissionType permission)
        {
            List<string> users = GetUsersWithPermissions(listSyncId, ShoppingListPermissionType.Read);
            await _hubContext.Clients.Users(users).SendAsync("ListRemoved", listSyncId);
        }

        private async Task SendListRemoved(string listSyncId, string userId)
        {
            await _hubContext.Clients.Users(userId).SendAsync("ListRemoved", listSyncId);
        }

        // Inform the given user that its permission for the given list changed.
        private async Task SendListPermissionChanged(
            string listSyncId,
            string userId,
            ShoppingListPermissionType permission)
        {
            await _hubContext.Clients.Users(userId).SendAsync("ListPermissionChanged", listSyncId, permission);
        }

        private async Task SendItemNameChanged(
            string newItemName,
            string oldItemName,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            List<string> users = GetUsersWithPermissions(listSyncId, permission);
            await _hubContext.Clients.Users(users).SendAsync("ItemNameChanged", listSyncId, newItemName, oldItemName);
        }

        private async Task SendItemAddedOrUpdated(
            GenericItem item,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            string itemJson = JsonConvert.SerializeObject(item);
            List<string> users = GetUsersWithPermissions(listSyncId, permission);
            await _hubContext.Clients.Users(users).SendAsync("ItemAddedOrUpdated", listSyncId, itemJson);
        }

        private async Task SendProductAddedOrUpdated(
            GenericProduct product,
            string listSyncId,
            ShoppingListPermissionType permission)
        {
            string productJson = JsonConvert.SerializeObject(product);
            List<string> users = GetUsersWithPermissions(listSyncId, permission);
            await _hubContext.Clients.Users(users).SendAsync("ProductAddedOrUpdated", listSyncId, productJson);
        }
    }
}