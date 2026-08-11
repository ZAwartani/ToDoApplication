using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class RestoreSeedData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
    table: "Categories",
    columns: new[] { "Id", "TitleOfCategory" },
    values: new object[] { 1, "Api" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
       table: "Categories",
       keyColumn: "Id",
       keyValue: 1);
        }
    }
}
