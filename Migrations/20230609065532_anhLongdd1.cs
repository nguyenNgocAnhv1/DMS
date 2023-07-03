using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class anhLongdd1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListName",
                table: "BoxShares");

            migrationBuilder.DropColumn(
                name: "BanDate",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Boxs",
                newName: "IsAvaliable");

            migrationBuilder.AddColumn<int>(
                name: "View",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BoxShares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoxShares_UserId",
                table: "BoxShares",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxShares_Accounts_UserId",
                table: "BoxShares",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxShares_Accounts_UserId",
                table: "BoxShares");

            migrationBuilder.DropIndex(
                name: "IX_BoxShares_UserId",
                table: "BoxShares");

            migrationBuilder.DropColumn(
                name: "View",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BoxShares");

            migrationBuilder.RenameColumn(
                name: "IsAvaliable",
                table: "Boxs",
                newName: "IsPublic");

            migrationBuilder.AddColumn<string>(
                name: "ListName",
                table: "BoxShares",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BanDate",
                table: "Accounts",
                type: "datetime2",
                nullable: true);
        }
    }
}
