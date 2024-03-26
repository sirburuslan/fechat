using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class _025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "Expiration",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                defaultValue: 0,
                oldDefaultValue: 0,
                oldNullable: true,
                oldComment: "Previous comment",
                comment: "New comment"
            );

            migrationBuilder.AlterColumn<int>(
                name: "ResetTime",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Expiration",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                defaultValue: 0,
                oldDefaultValue: 0,
                oldNullable: true,
                oldComment: "Previous comment",
                comment: "New comment"
            );

            migrationBuilder.AlterColumn<int>(
                name: "ResetTime",
                table: "Members",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
