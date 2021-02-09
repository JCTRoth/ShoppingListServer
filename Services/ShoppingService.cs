using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        bool UpdateList(ShoppingList shoppingList);
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

            // TO DO Get User from Token
            // GET TOKEN FROM REQUEST
            // OR GET BY OWNER ID ON LIST??
            // int pos_key = Program._users.IndexOf(User => User.Token == );

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
                Program._shoppingLists.Add(new_list_item);
                Json_Files.Store_ShoppingList(new_list_item.OwnerID, new_list_item);
            }

            return true;
        }



        public bool UpdateList(ShoppingList new_list_item)
        {

            // TO DO Replace by DB
            // Add to list of shoppingLists
            bool is_in_list = Program._shoppingLists.Any(ShoppingList => ShoppingList.SyncID == new_list_item.SyncID);

            if (!is_in_list)
            {
                // already in List
                return false;
            }
            else
            {
                // TO DO Check if user is allowed to edit list
                // TO DO Implement UPDATE METHOD
                return true;
            }

        }

        public bool DeleteList(string userID,int del_syncID)
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
    }
}