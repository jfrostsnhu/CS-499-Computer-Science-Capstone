using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class NotificationItemRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LowStockNotifications_ItemId",
                schema: "dbo",
                table: "LowStockNotifications",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_LowStockNotifications_Item_ItemId",
                schema: "dbo",
                table: "LowStockNotifications",
                column: "ItemId",
                principalSchema: "dbo",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LowStockNotifications_Item_ItemId",
                schema: "dbo",
                table: "LowStockNotifications");

            migrationBuilder.DropIndex(
                name: "IX_LowStockNotifications_ItemId",
                schema: "dbo",
                table: "LowStockNotifications");
        }
    }
}
