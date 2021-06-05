﻿using System;
using System.IO;
using Newtonsoft.Json;
using ShoppingListServer.Models;

namespace ShoppingListServer.Logic
{
    public class Json_Files
    {
        // TODO USER fastJSON

        Folder _folder_service;
        public Json_Files()
        {
            _folder_service = new Folder();
        }

        public ShoppingList Load_ShoppingList(string user_id, string shoppingList_id)
        {
            ShoppingList list = null;
            try
            {
                string file_path =
                    System.IO.Path.Combine(_folder_service.Get_User_Folder_Path(user_id), shoppingList_id + ".json");

                if (File.Exists(file_path))
                {
                    string file_content = File.ReadAllText(file_path);
                    list = JsonConvert.DeserializeObject<ShoppingList>(file_content);
                }
                else
                {
                    Console.Error.WriteLine("Load_ShoppingList: list not found at " + file_path);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Load_ShoppingList " + ex);
            }
            return list;
        }

        public bool Store_ShoppingList(string user_id, ShoppingList shoppingList)
        {
            try
            {
                string folder_path = _folder_service.Get_User_Folder_Path(user_id);
                string file_path = System.IO.Path.Combine(folder_path, shoppingList.SyncId + ".json");
                string list_as_string = JsonConvert.SerializeObject(shoppingList);

                if (!System.IO.Directory.Exists(folder_path))
                {
                    System.IO.Directory.CreateDirectory(folder_path);
                }

                File.WriteAllText(file_path, list_as_string);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Store_ShoppingList " + ex);
                return false;
            }
        }

        public bool Update_ShoppingList(string user_id, ShoppingList shoppingList)
        {
            try
            {
                string folder_path = _folder_service.Get_User_Folder_Path(user_id);
                string file_path = System.IO.Path.Combine(folder_path, shoppingList.SyncId + ".json");
                string list_as_string = JsonConvert.SerializeObject(shoppingList);

                if (!System.IO.Directory.Exists(folder_path))
                {
                    return false;
                }

                if (!System.IO.File.Exists(file_path))
                {
                    return false;
                }

                File.WriteAllText(file_path, list_as_string);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Update_ShoppingList " + ex);
                return false;
            }
        }

        public bool Delete_ShoppingList(string user_id, string shoppingList_id)
        {
            try
            {
                string file_path = System.IO.Path.Combine(_folder_service.Get_User_Folder_Path(user_id),
                                                                        shoppingList_id + ".json");

                if (File.Exists(file_path))
                {
                    File.Delete(file_path);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Delete_ShoppingList " + ex);
                return false;
            }
        }


    }
}
