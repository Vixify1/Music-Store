using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddedWishlistEssentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Wishlist",
            //    columns: table => new
            //    {
            //        WishlistId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CustomerId = table.Column<int>(type: "int", nullable: false),
            //        AlbumId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Wishlist", x => x.WishlistId);
            //        table.ForeignKey(
            //            name: "FK_Wishlist_Album_AlbumId",
            //            column: x => x.AlbumId,
            //            principalTable: "Album",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Wishlist_Customer_CustomerId",
            //            column: x => x.CustomerId,
            //            principalTable: "Customer",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Wishlist_AlbumId",
            //    table: "Wishlist",
            //    column: "AlbumId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Wishlist_CustomerId",
            //    table: "Wishlist",
            //    column: "CustomerId");


            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wishlist')
    BEGIN
        CREATE TABLE [Wishlist] (
            [WishlistId] INT NOT NULL IDENTITY(1,1),
            [CreatedAt] DATETIME2 NOT NULL,
            [CustomerId] INT NOT NULL,
            [AlbumId] INT NOT NULL,
            CONSTRAINT [PK_Wishlist] PRIMARY KEY ([WishlistId]),
            CONSTRAINT [FK_Wishlist_Album_AlbumId] FOREIGN KEY ([AlbumId]) REFERENCES [Album] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [FK_Wishlist_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customer] ([Id]) ON DELETE CASCADE
        );

        CREATE INDEX [IX_Wishlist_AlbumId] ON [Wishlist] ([AlbumId]);
        CREATE INDEX [IX_Wishlist_CustomerId] ON [Wishlist] ([CustomerId]);
    END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wishlist");
        }
    }
}
