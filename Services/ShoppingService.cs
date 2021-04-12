using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShoppingListServer.Database;
using ShoppingListServer.Entities;
using ShoppingListServer.Exceptions;
using ShoppingListServer.Helpers;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;

namespace ShoppingListServer.Services
{
    public interface IShoppingService
    {
        string GetID();
        ShoppingList GetList(string userId, string shoppingListId);
        bool AddList(ShoppingList list, string userID);
        bool Update_Item_In_List(string itemNameOld, GenericProduct itemNew, string userId, string shoppingListId);
        bool Remove_Item_In_List(string itemNameOld, string userId, string shoppingListId);
        bool Add_Item_To_List(GenericProduct newItem, string userId, string shoppingListId);
        bool DeleteList(string userId, string shoppingListId);
    }

    public class ShoppingService : IShoppingService
    {
        private readonly AppSettings _appSettings;
        private readonly Json_Files _json_files;
        private readonly AppDb _db;

        public ShoppingService(IOptions<AppSettings> appSettings, AppDb db)
        {
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
            CheckPermission(list, userId, ShoppingListPermissionType.Read);
            return _json_files.Load_ShoppingList(userId, shoppingListId);
        }

        // Adds the given list to this server.
        // Sets list.Id and list.ShoppingListPermissions
        public bool AddList(ShoppingList list, string userID)
        {
            ShoppingList existingList = GetShoppingListEntity(list.Id);
            if (existingList != null)
            {
                return false;
            }
            else
            {
                list.Id = Guid.NewGuid().ToString();
                list.ShoppingListPermissions = new List<ShoppingListPermission>();
                list.ShoppingListPermissions.Add(new ShoppingListPermission()
                {
                    PermissionType = ShoppingListPermissionType.All,
                    ShoppingList = list,
                    UserId = userID
                });
                
                if (_json_files.Store_ShoppingList(list.ShoppingListPermissions.FirstOrDefault().UserId, list))
                {
                    _db.ShoppingLists.Add(list);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeleteList(string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermission(listEntity, userId, ShoppingListPermissionType.Delete);
            _db.ShoppingLists.Remove(listEntity);
            _db.SaveChanges();
            _json_files.Delete_ShoppingList(userId, shoppingListId);
            return true;
        }

        public bool Update_Item_In_List(string itemNameOld, GenericProduct itemNew, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermission(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = _json_files.Load_ShoppingList(userId, shoppingListId);

            int index = list.ProductList.FindIndex(prod => prod.Item.Name == itemNameOld);
            if (index != -1)
            {
                list.ProductList[index] = itemNew;
            }
            else
            {
                list.ProductList.Add(itemNew);
            }

            return _json_files.Update_ShoppingList(userId, list);
        }

        public bool Remove_Item_In_List(string itemNameOld, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermission(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = _json_files.Load_ShoppingList(userId, shoppingListId);

            int index = list.ProductList.FindIndex(prod => prod.Item.Name == itemNameOld);
            if (index != -1)
            {
                list.ProductList.RemoveAt(index);
                return _json_files.Update_ShoppingList(userId, list);
            }
            return false;
        }

        public bool Add_Item_To_List(GenericProduct newItem, string userId, string shoppingListId)
        {
            ShoppingList listEntity = GetShoppingListEntity(shoppingListId);
            if (listEntity == null)
                throw new ShoppingListNotFoundException(shoppingListId);
            CheckPermission(listEntity, userId, ShoppingListPermissionType.Write);
            ShoppingList list = _json_files.Load_ShoppingList(userId, shoppingListId);

            int index = list.ProductList.FindIndex(prod => prod.Item.Name == newItem.Item.Name);
            if (index == -1)
            {
                list.ProductList[index] = newItem;
                return _json_files.Update_ShoppingList(userId, list);
            }
            return false;
        }

        private ShoppingList GetShoppingListEntity(string shoppingListId)
        {
            return _db.ShoppingLists.FirstOrDefault(ShoppingList => ShoppingList.Id == shoppingListId);
        }

        private void CheckPermission(ShoppingList list, string userId, ShoppingListPermissionType expectedPermission)
        {
            var permissions = list.ShoppingListPermissions
                .Where(per => per.UserId == userId && per.PermissionType.HasFlag(expectedPermission))
                .ToList();
            ShoppingListPermission permission = permissions.FirstOrDefault();                
                
                //(from per in list.ShoppingListPermissions
                // where per.UserId == userId && per.PermissionType.HasFlag(expectedPermission)
                // select per).FirstOrDefault();

            if (permission == null)
                throw new NoShoppingListPermissionException(permission, expectedPermission);
        }
    }
}