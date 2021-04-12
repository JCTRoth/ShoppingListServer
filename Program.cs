using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer
{
    public class Program
    {
        // TODO Replace Config by Config Service
        public static string _data_storage_folder;

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
