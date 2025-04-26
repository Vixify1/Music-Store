using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Model.Migrations
{
    /// <inheritdoc />
    public partial class checkthegenre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumItem_Album_AlbumId",
                table: "AlbumItem");

            migrationBuilder.DropIndex(
                name: "IX_AlbumItem_AlbumId",
                table: "AlbumItem");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "AlbumItem");

            migrationBuilder.RenameColumn(
                name: "genreID",
                table: "Genre",
                newName: "GenreId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AlbumItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "AlbumItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "AlbumItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(22,6)",
                oldPrecision: 22,
                oldScale: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "Genre",
                newName: "genreID");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AlbumItem",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "AlbumItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AlbumItem",
                type: "decimal(22,6)",
                precision: 22,
                scale: 6,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "AlbumItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlbumItem_AlbumId",
                table: "AlbumItem",
                column: "AlbumId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumItem_Album_AlbumId",
                table: "AlbumItem",
                column: "AlbumId",
                principalTable: "Album",
                principalColumn: "Id");
        }
    }
}
