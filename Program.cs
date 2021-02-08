using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShoppingListServer.Entities;
using ShoppingListServer.Logic;
using System.Collections.Generic;

namespace ShoppingListServer
{
    public class Program
    {
        // TO DO Replace Config by Config Service
        public static string _data_storage_folder;

        // TO DO Replace by DataBase
        public static List<User> _users = new List<User>();
        public static List<ShoppingList> _shoppingLists = new List<ShoppingList>();
        public static List<string> _syncIDs = new List<string>();

        public static void Main(string[] args)
        {
            // Create APIs storage folder
            Folder.Create_Data_Storage_Folder();

            // Add Test Users (Debug Mode Only)
#if DEBUG
            // They have no folders!
            _users.Add(new User { Id = "1", EMail = "admin@mailbase.info", Password = "admin", Role = Role.Admin });
            _users.Add(new User { Id = "2", EMail = "user@mailbase.info", Password = "user", Role = Role.User });
#endif
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://localhost:4000");
                });
    }
}
