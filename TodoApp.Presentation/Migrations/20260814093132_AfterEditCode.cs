using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class AfterEditCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads");

            migrationBuilder.RenameColumn(
                name: "AddressNameFile",
                table: "FilesUploads",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "Codeotp",
                table: "codeOTPs",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "CodeOTPExpir",
                table: "codeOTPs",
                newName: "CodeExpirDate");

            migrationBuilder.RenameColumn(
                name: "TitleOfCategory",
                table: "Categories",
                newName: "Title");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FilesUploads_Path",
                table: "FilesUploads",
                column: "Path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilesUploads_Path",
                table: "FilesUploads");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tokens");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "FilesUploads",
                newName: "AddressNameFile");

            migrationBuilder.RenameColumn(
                name: "CodeExpirDate",
                table: "codeOTPs",
                newName: "CodeOTPExpir");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "codeOTPs",
                newName: "Codeotp");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Categories",
                newName: "TitleOfCategory");

            migrationBuilder.CreateIndex(
                name: "IX_FilesUploads_AddressNameFile",
                table: "FilesUploads",
                column: "AddressNameFile",
                unique: true);
        }
    }
}
