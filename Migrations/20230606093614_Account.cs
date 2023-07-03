using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BanDate",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BanEnabled",
                table: "Accounts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "BanEnabled",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Accounts");
        }
    }
}
