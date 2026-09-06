using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToFilesUploadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackGroundJobId",
                table: "ToDoItems");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FilesUploads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FilesUploads_UserId",
                table: "FilesUploads",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesUploads_AspNetUsers_UserId",
                table: "FilesUploads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesUploads_AspNetUsers_UserId",
                table: "FilesUploads");

            migrationBuilder.DropIndex(
                name: "IX_FilesUploads_UserId",
                table: "FilesUploads");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FilesUploads");

            migrationBuilder.AddColumn<string>(
                name: "BackGroundJobId",
                table: "ToDoItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
