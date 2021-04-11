using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingListServer.Entities;
using ShoppingListServer.Models;
using ShoppingListServer.Models.ShoppingData;

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
        public DbSet<ShoppingList> ShoppingLists { get; set; }

        public AppDb(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // see "Indirect many-to-many relationships" in
            // https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
            // ShoppingListPermission is a n:n relationship between ShoppingList and User.
            // The indirect method is used here so it's possible to add the "PermissionType" property to the ShoppingListPermission relationship.
            modelBuilder.Entity<ShoppingListPermission>()
                .HasKey(accessRight => new { accessRight.ShoppingListId, accessRight.UserId});

            modelBuilder.Entity<ShoppingListPermission>()
                .HasOne(accessRight => accessRight.ShoppingList)
                .WithMany(shoppingList => shoppingList.ShoppingListPermissions)
                .HasForeignKey(accessRight => accessRight.ShoppingListId);

            modelBuilder.Entity<ShoppingListPermission>()
                .HasOne(accessRight => accessRight.User)
                .WithMany(user => user.ShoppingListPermissions)
                .HasForeignKey(accessRight => accessRight.UserId);
        }
    }
}
