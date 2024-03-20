using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class _007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PlansRestrictions",
                newName: "RestrictionValue");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PlansRestrictions",
                newName: "RestrictionName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RestrictionValue",
                table: "PlansRestrictions",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "RestrictionName",
                table: "PlansRestrictions",
                newName: "Name");
        }
    }
}
