using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToeApp.API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Board = table.Column<string>(type: "text", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentMove = table.Column<string>(type: "text", nullable: false),
                    CurrentStep = table.Column<long>(type: "bigint", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Age", "Board", "CreatedAt", "CurrentMove", "CurrentStep", "Result", "Status" },
                values: new object[] { new Guid("b8aa36a5-9094-4dbd-80a5-444fcb92d3a4"), 0, "[[\"X\",\"O\",null],[null,\"X\",\"O\"],[\"O\",null,\"X\"]]", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "X", 0L, "None", "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
