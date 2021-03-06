﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingListServer.Database;

namespace ShoppingListServer.Migrations
{
    [DbContext(typeof(AppDb))]
    partial class AppDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "4.0.5");

            modelBuilder.Entity("ShoppingListServer.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("EMail")
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("Salt")
                        .HasMaxLength(16)
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("Token")
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ShoppingListServer.Models.ShoppingData.ShoppingListPermission", b =>
                {
                    b.Property<string>("ShoppingListId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("PermissionType")
                        .HasColumnType("int");

                    b.HasKey("ShoppingListId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingListPermission");
                });

            modelBuilder.Entity("ShoppingListServer.Models.ShoppingList", b =>
                {
                    b.Property<string>("SyncId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("SyncId");

                    b.ToTable("ShoppingLists");
                });

            modelBuilder.Entity("ShoppingListServer.Models.ShoppingData.ShoppingListPermission", b =>
                {
                    b.HasOne("ShoppingListServer.Models.ShoppingList", "ShoppingList")
                        .WithMany("ShoppingListPermissions")
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShoppingListServer.Entities.User", "User")
                        .WithMany("ShoppingListPermissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShoppingList");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShoppingListServer.Entities.User", b =>
                {
                    b.Navigation("ShoppingListPermissions");
                });

            modelBuilder.Entity("ShoppingListServer.Models.ShoppingList", b =>
                {
                    b.Navigation("ShoppingListPermissions");
                });
#pragma warning restore 612, 618
        }
    }
}
