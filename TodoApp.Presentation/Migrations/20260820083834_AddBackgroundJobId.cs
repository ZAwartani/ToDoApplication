using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackGroundJobId",
                table: "ToDoItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackGroundJobId",
                table: "ToDoItems");
        }
    }
}
