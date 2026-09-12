using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class removeissend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSend",
                table: "RecurringTodos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSend",
                table: "RecurringTodos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
