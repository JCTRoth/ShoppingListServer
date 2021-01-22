using System;

namespace ShoppingListServer.Logic
{
    public class Folder
    {

        public static string Get_User_Folder_Path(string user_id)
        {   
            // TO DO Replace by config
            string main_folder = Program._data_storage_folder;
            
            return System.IO.Path.Combine(main_folder, user_id);
        }


        // Create the folder where user shoppinglists stored on
        public static bool Create_User_Folder(string user_id)
        {
            string full_path = System.IO.Path.Combine(Get_User_Folder_Path(user_id), user_id);

            return Create_Folder(full_path);
        }


        // Creates the Data Storage Folder where JSON Shopping List's are placed in
        // After Creation set's config variable
        public static bool Create_Data_Storage_Folder(string new_folder = "Shopping_List_Server_Data")
        {
            string full_path = System.IO.Path.Combine(Get_Home_Folder(), new_folder);

            if (Create_Folder(full_path))
            {
                Program._data_storage_folder = full_path;
                return true;
            }

            return false;
        }


        // False if exist or not able to create.
        private static bool Create_Folder(string folder_path)
        {
            try
            {
                if (!System.IO.Directory.Exists(folder_path))
                {
                    System.IO.Directory.CreateDirectory(folder_path);

                    if (System.IO.Directory.Exists(folder_path))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Create_Data_Storage_Folder " + ex);
            }

            return false;
        }


        // Returns Home Folder Path
        public static string Get_Home_Folder()
        {
            string home_folder;

            // C# see's Linux also as Unix 
            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                home_folder = Environment.GetEnvironmentVariable("HOME");
            }
            else // Windows
            {
                home_folder = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }

            return home_folder;
        }


    }
}
