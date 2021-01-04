using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShoppingListServer.Entities;
using System.Collections.Generic;

namespace ShoppingListServer
{
    public class Program
    {
        // TO DO Replace by DataBase
        public static List<User> _users = new List<User>();

        public static void Main(string[] args)
        {

            // Add Test Users (Debug Mode Only)
#if DEBUG
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
