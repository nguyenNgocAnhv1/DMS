using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class Update_Box_DateCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxs_Accounts_UserId",
                table: "Boxs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Boxs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Boxs",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Boxs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boxs_Accounts_UserId",
                table: "Boxs",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxs_Accounts_UserId",
                table: "Boxs");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Boxs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Boxs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Boxs",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxs_Accounts_UserId",
                table: "Boxs",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
