using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniqloMVC.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "ProductComments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ProductComments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductComments");
        }
    }
}
