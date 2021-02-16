using System;
using System.IO;
using System.Text.Unicode;
using Newtonsoft.Json;
using ShoppingListServer.Entities;

namespace ShoppingListServer.Logic
{
    public class Json_Files
    {
        // USE SERILZED FORMAT TO STORE INFO ???

        public static ShoppingList Load_ShoppingList(string user_id, int shoppingList_id)
        {
            try
            {
                string file_path = System.IO.Path.Combine(Folder.Get_User_Folder_Path(user_id),
                                                                        shoppingList_id + ".json");

                string file_content = File.ReadAllText(file_path);
                ShoppingList loaded_shoppinglist = JsonConvert.DeserializeObject<ShoppingList>(file_content);

                return loaded_shoppinglist;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Load_ShoppingList " + ex);
                return new ShoppingList();
            }
        }

        public static bool Store_ShoppingList(string user_id, ShoppingList shoppingList)
        {
            try
            {
                string folder_path = Folder.Get_User_Folder_Path(user_id);
                string file_path = System.IO.Path.Combine(folder_path, shoppingList.SyncID + ".json");
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

        public static bool Delete_ShoppingList(string user_id, int shoppingList_id)
        {
            try
            {
                string file_path = System.IO.Path.Combine(Folder.Get_User_Folder_Path(user_id),
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
