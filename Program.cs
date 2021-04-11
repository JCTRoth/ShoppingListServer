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

        public static void Main(string[] args)
        {
            // Create APIs storage folder
            Folder.Create_Data_Storage_Folder();

            // Add Test Users (Debug Mode Only)
            //// They have no folders!
            //_users.Add(new User { Id = "1", EMail = "admin@mailbase.info", Password = "admin", Role = Role.Admin });
            //_users.Add(new User { Id = "2", EMail = "user@mailbase.info", Password = "user", Role = Role.User });
            
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
