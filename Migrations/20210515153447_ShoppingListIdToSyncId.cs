using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingListServer.Migrations
{
    public partial class ShoppingListIdToSyncId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingLists",
                newName: "SyncId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SyncId",
                table: "ShoppingLists",
                newName: "Id");
        }
    }
}
