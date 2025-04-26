using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Model.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAlbumDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "Genre",
                newName: "genreID");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Album",
                newName: "coverUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "Album",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "Album",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AlbumItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(22,6)", precision: 22, scale: 6, nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumItem_Album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Album",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumItem_AlbumId",
                table: "AlbumItem",
                column: "AlbumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumItem");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "Album");

            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "Album");

            migrationBuilder.RenameColumn(
                name: "genreID",
                table: "Genre",
                newName: "GenreId");

            migrationBuilder.RenameColumn(
                name: "coverUrl",
                table: "Album",
                newName: "ImageUrl");
        }
    }
}
