using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingListServer.Entities;
using ShoppingListServer.Models;

namespace ShoppingListServer.Database
{
    // Database in Asp.Net Core:
    //
    // Create all tables (see https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli):
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    // 
    // Tutorials:
    // - https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    // - https://docs.microsoft.com/en-us/ef/core/modeling/
    // 
    public class AppDb : DbContext
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<string> SyncIDs { get; set; }

        public AppDb(DbContextOptions options) : base(options)
        {
        }
    }
}
