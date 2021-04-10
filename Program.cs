using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer
{
    public class Program
    {
        // TO DO Replace Config by Config Service
        public static string _data_storage_folder;

        // TO DO Replace by DataBase
        public static List<ShoppingList> _shoppingLists = new List<ShoppingList>();
        public static List<string> _syncIDs = new List<string>();

        public static void Main(string[] args)
        {
            // Create APIs storage folder
            new Folder().Create_Data_Storage_Folder();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:5678");
                });
    }
}
