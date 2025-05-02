//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace MusicStore.Model.Migrations
//{
//    /// <inheritdoc />
//    public partial class OrderItems : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//        //migrationBuilder.RenameColumn(
//        //    name: "updatedAt",
//        //    table: "Order",
//        //    newName: "UpdatedAt");

//        //migrationBuilder.RenameColumn(
//        //    name: "totalAmount",
//        //    table: "Order",
//        //    newName: "TotalAmount");

//        //migrationBuilder.RenameColumn(
//        //    name: "status",
//        //    table: "Order",
//        //    newName: "Status");

//        //migrationBuilder.RenameColumn(
//        //    name: "orderDate",
//        //    table: "Order",
//        //    newName: "OrderDate");

//        //migrationBuilder.RenameColumn(
//        //    name: "createdAt",
//        //    table: "Order",
//        //    newName: "CreatedAt");

//        //migrationBuilder.CreateTable(
//        //    name: "OrderItems",
//        //    columns: table => new
//        //    {
//        //        OrderItemId = table.Column<int>(type: "int", nullable: false)
//        //            .Annotation("SqlServer:Identity", "1, 1"),
//        //        OrderId = table.Column<int>(type: "int", nullable: false),
//        //        Id = table.Column<int>(type: "int", nullable: false),
//        //        Quantity = table.Column<int>(type: "int", nullable: false),
//        //        UnitPrice = table.Column<decimal>(type: "decimal(22,6)", precision: 22, scale: 6, nullable: false),
//        //        Subtotal = table.Column<decimal>(type: "decimal(22,6)", precision: 22, scale: 6, nullable: false)
//        //    },
//        //constraints: table =>
//        //{
//        //    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
//        //    table.ForeignKey(
//        //        name: "FK_OrderItems_Album_Id",
//        //        column: x => x.Id,
//        //        principalTable: "Album",
//        //        principalColumn: "Id",
//        //        onDelete: ReferentialAction.Cascade);
//        //    table.ForeignKey(
//        //        name: "FK_OrderItems_Order_OrderId",
//        //        column: x => x.OrderId,
//        //        principalTable: "Order",
//        //        principalColumn: "OrderId",
//        //        onDelete: ReferentialAction.Cascade);
//        //        });

//            migrationBuilder.Sql(@"
//        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
//        BEGIN
//            CREATE TABLE [OrderItems] (
//                [OrderItemId] int NOT NULL IDENTITY(1,1),
//                [OrderId] int NOT NULL,
//                [Id] int NOT NULL,
//                [Quantity] int NOT NULL,
//                [UnitPrice] decimal(22,6) NOT NULL,
//                [Subtotal] decimal(22,6) NOT NULL,
//                CONSTRAINT [PK_OrderItems] PRIMARY KEY ([OrderItemId]),
//                CONSTRAINT [FK_OrderItems_Album_Id] FOREIGN KEY ([Id]) REFERENCES [Album] ([Id]) ON DELETE CASCADE,
//                CONSTRAINT [FK_OrderItems_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([OrderId]) ON DELETE CASCADE
//            )
//        END
//    ");

//            //migrationBuilder.CreateIndex(
//            //    name: "IX_Order_CustomerId",
//            //    table: "Order",
//            //    column: "CustomerId");

//            //migrationBuilder.CreateIndex(
//            //    name: "IX_Cart_CustomerId",
//            //    table: "Cart",
//            //    column: "CustomerId");

//            //migrationBuilder.CreateIndex(
//            //    name: "IX_OrderItems_Id",
//            //    table: "OrderItems",
//            //    column: "Id");

//            //migrationBuilder.CreateIndex(
//            //    name: "IX_OrderItems_OrderId",
//            //    table: "OrderItems",
//            //    column: "OrderId");

//            //migrationBuilder.AddForeignKey(
//            //    name: "FK_Cart_Customer_CustomerId",
//            //    table: "Cart",
//            //    column: "CustomerId",
//            //    principalTable: "Customer",
//            //    principalColumn: "Id",
//            //    onDelete: ReferentialAction.Cascade);

//            //migrationBuilder.AddForeignKey(
//            //    name: "FK_Order_Customer_CustomerId",
//            //    table: "Order",
//            //    column: "CustomerId",
//            //    principalTable: "Customer",
//            //    principalColumn: "Id",
//            //    onDelete: ReferentialAction.Cascade);
//        }


//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_Cart_Customer_CustomerId",
//                table: "Cart");

//            migrationBuilder.DropForeignKey(
//                name: "FK_Order_Customer_CustomerId",
//                table: "Order");

//            migrationBuilder.DropTable(
//                name: "OrderItems");

//            migrationBuilder.DropIndex(
//                name: "IX_Order_CustomerId",
//                table: "Order");

//            migrationBuilder.DropIndex(
//                name: "IX_Cart_CustomerId",
//                table: "Cart");

//            migrationBuilder.RenameColumn(
//                name: "UpdatedAt",
//                table: "Order",
//                newName: "updatedAt");

//            migrationBuilder.RenameColumn(
//                name: "TotalAmount",
//                table: "Order",
//                newName: "totalAmount");

//            migrationBuilder.RenameColumn(
//                name: "Status",
//                table: "Order",
//                newName: "status");

//            migrationBuilder.RenameColumn(
//                name: "OrderDate",
//                table: "Order",
//                newName: "orderDate");

//            migrationBuilder.RenameColumn(
//                name: "CreatedAt",
//                table: "Order",
//                newName: "createdAt");
//        }
//    }
//}


using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Model.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderItemAlbumId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, drop the existing foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Album_Id",
                table: "OrderItems");

            // Drop the index on the old column
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_Id",
                table: "OrderItems");

            // Rename the column from "Id" to "AlbumId"
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderItems",
                newName: "AlbumId");

            // Create a new index on the renamed column
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_AlbumId",
                table: "OrderItems",
                column: "AlbumId");

            // Add the foreign key constraint back with the new column name
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Album_AlbumId",
                table: "OrderItems",
                column: "AlbumId",
                principalTable: "Album",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert all changes if needed to roll back
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Album_AlbumId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_AlbumId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "AlbumId",
                table: "OrderItems",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Id",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Album_Id",
                table: "OrderItems",
                column: "Id",
                principalTable: "Album",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}