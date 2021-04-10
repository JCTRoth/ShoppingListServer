using System;
using System.Linq;
using Microsoft.Extensions.Options;
using ShoppingListServer.Database;
using ShoppingListServer.Entities;
using ShoppingListServer.Helpers;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer.Services
{
    public interface IShoppingService
    {
        Result GetList(string userID, string syncID);
        Result AddList(ShoppingList shoppingList);
        bool Update_Item_In_List(string name_of_old_item, GenericProduct Old_Item, ShoppingList shoppingList);
        bool Remove_Item_In_List(string name_of_old_item, ShoppingList shoppingList);
        bool Add_Item_To_List(GenericProduct new_item, ShoppingList shoppingList);
        bool DeleteList(string userID, string del_syncID);
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
            // We hope it's unique enough
            string new_id = Guid.NewGuid().ToString();

            return new_id;
        }

        public Result GetList(string userID, string syncID)
        {
            Result result = new Result();
            ShoppingList list = _json_files.Load_ShoppingList(userID, syncID);

            if (list.SyncID != "non")
            {
                result.WasFound = true;
                result.ReturnValue = list;
            }
            else
            {
                result.ReturnValue = "";
                result.WasFound = false;
            }

            return result;
        }

        public Result AddList(ShoppingList new_list_item)
        {
            Result result = new Result();

            // new SyncID
            new_list_item.SyncID = this.GetID();

            if (_json_files.Store_ShoppingList(new_list_item.OwnerID, new_list_item))
            {
                result.ReturnValue = new_list_item.SyncID;
                result.WasFound = true;
                return result;
            }
            else
            {
                result.WasFound = false;
                return result;
            }
        }

        public bool DeleteList(string userID, string del_syncID)
        {
            if (_json_files.Delete_ShoppingList(userID, del_syncID))
            {
                return true;
            }

            return false;
        }

        public bool Update_Item_In_List(string name_of_old_item, GenericProduct New_Item, ShoppingList shoppingList)
        {
            // Names in ShoppingLists are Unique
            for (int i = 0; i < shoppingList.ProductList.Count; i++)
            {
                if (shoppingList.ProductList[i].Item.Name == name_of_old_item)
                {
                    shoppingList.ProductList[i] = New_Item;
                    _json_files.Update_ShoppingList(shoppingList.OwnerID, shoppingList);
                    return true;
                }
            }

            return false;
        }

        public bool Remove_Item_In_List(string name_of_old_item, ShoppingList shoppingList)
        {
            try
            {
                shoppingList.ProductList.RemoveAll(x => x.Item.Name == name_of_old_item);
                _json_files.Update_ShoppingList(shoppingList.OwnerID, shoppingList);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remove_Item_In_List " + ex);
            }

            return false;
        }

        public bool Add_Item_To_List(GenericProduct new_item, ShoppingList shoppingList)
        {
            try
            {
                shoppingList.ProductList.Add(new_item);
                _json_files.Update_ShoppingList(shoppingList.OwnerID, shoppingList);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add_Item_To_List " + ex);
            }

            return false;
        }
    }
}