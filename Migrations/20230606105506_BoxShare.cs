using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class BoxShare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoxShares",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListName = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    BoxId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxShares", x => x.id);
                    table.ForeignKey(
                        name: "FK_BoxShares_Boxs_BoxId",
                        column: x => x.BoxId,
                        principalTable: "Boxs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxShares_BoxId",
                table: "BoxShares",
                column: "BoxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoxShares");
        }
    }
}
