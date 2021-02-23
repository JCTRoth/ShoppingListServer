using System;
using System.Linq;
using Microsoft.Extensions.Options;
using ShoppingListServer.Entities;
using ShoppingListServer.Helpers;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer.Services
{
    public interface IShoppingService
    {
        string GetID();
        Result GetList(string userID, int syncID);
        bool AddList(ShoppingList shoppingList);
        bool Update_Item_In_List(string name_of_old_item, Item Old_Item, ShoppingList shoppingList);
        bool Remove_Item_In_List(string name_of_old_item, ShoppingList shoppingList);
        bool Add_Item_To_List(Item new_item, ShoppingList shoppingList);
        bool DeleteList(string userID, int del_syncID);
    }

    public class ShoppingService : IShoppingService
    {
        private readonly AppSettings _appSettings;

        public ShoppingService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public string GetID()
        {
            string new_id = Guid.NewGuid().ToString();

            // TO DO REPLACE
            Program._syncIDs.Add(new_id);

            return new_id;
        }

        public Result GetList(string userID, int syncID)
        {
            Result result = new Result();

            // TO DO Check if user is allowed
            int index = Program._shoppingLists.FindIndex(ShoppingList => ShoppingList.SyncID == syncID);
            if (index != -1)
            {
                result.WasFound = true;
                result.ReturnValue = Json_Files.Load_ShoppingList(userID, syncID);
            }
            else
            {
                result.ReturnValue = "";
                result.WasFound = false;
            }

            return result;
        }


        public bool AddList(ShoppingList new_list_item)
        {
            // TO DO Replace by DB
            // Add to list of shoppingLists
            bool is_in_list = Program._shoppingLists.Any(ShoppingList => ShoppingList.SyncID == new_list_item.SyncID);

            if (is_in_list)
            {
                // already in List
                return false;
            }
            else
            {
                // TO DO OwnerID
                if (Json_Files.Store_ShoppingList(new_list_item.OwnerID, new_list_item))
                {
                    Program._shoppingLists.Add(new_list_item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public bool DeleteList(string userID, int del_syncID)
        {
            // TO DO Check if user is allowed

            int del_index = Program._shoppingLists.FindIndex(ShoppingList => ShoppingList.SyncID == del_syncID);
            if (del_index != -1)
            {
                // TO DO REPLACE
                Program._shoppingLists.RemoveAt(del_index);

                Json_Files.Delete_ShoppingList(userID, del_syncID);

                return true;
            }

            return false;
        }

        public bool Update_Item_In_List(string name_of_old_item, Item New_Item, ShoppingList shoppingList)
        {
            // Names in ShoppingLists are Unique
            for (int i = 0; i < shoppingList.ProductList.Count; i++)
            {
                if (shoppingList.ProductList[i].GenericItem.Name == name_of_old_item)
                {
                    shoppingList.ProductList[i] = New_Item;
                    Json_Files.Update_ShoppingList(shoppingList.OwnerID, shoppingList);
                    return true;
                }
            }
            
            return false;
        }

        public bool Remove_Item_In_List(string name_of_old_item, ShoppingList shoppingList)
        {
            // TO DO
            return true;
        }

        public bool Add_Item_To_List(Item new_item, ShoppingList shoppingList)
        {
            // TO DO
            return true;
        }
    }
}