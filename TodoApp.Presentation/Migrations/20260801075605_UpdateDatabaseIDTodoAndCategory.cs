using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseIDTodoAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesUploads_ToDoItems_TodoId",
                table: "FilesUploads");

            migrationBuilder.DropIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads");

            migrationBuilder.DropColumn(
                name: "fileUploadId",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "TodoId",
                table: "FilesUploads",
                newName: "ToDoItemId");

            migrationBuilder.RenameIndex(
                name: "IX_FilesUploads_TodoId",
                table: "FilesUploads",
                newName: "IX_FilesUploads_ToDoItemId");

            migrationBuilder.AlterColumn<string>(
                name: "AddressNameFile",
                table: "FilesUploads",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads",
                column: "AddressNameFile",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesUploads_ToDoItems_ToDoItemId",
                table: "FilesUploads",
                column: "ToDoItemId",
                principalTable: "ToDoItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesUploads_ToDoItems_ToDoItemId",
                table: "FilesUploads");

            migrationBuilder.DropIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads");

            migrationBuilder.RenameColumn(
                name: "ToDoItemId",
                table: "FilesUploads",
                newName: "TodoId");

            migrationBuilder.RenameIndex(
                name: "IX_FilesUploads_ToDoItemId",
                table: "FilesUploads",
                newName: "IX_FilesUploads_TodoId");

            migrationBuilder.AlterColumn<string>(
                name: "AddressNameFile",
                table: "FilesUploads",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "fileUploadId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "fileUploadId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads",
                column: "AddressNameFile",
                unique: true,
                filter: "[AddressNameFile] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesUploads_ToDoItems_TodoId",
                table: "FilesUploads",
                column: "TodoId",
                principalTable: "ToDoItems",
                principalColumn: "Id");
        }
    }
}
