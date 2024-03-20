using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class _011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RestrictionText",
                table: "PlansFeatures",
                newName: "FeatureText");

            migrationBuilder.RenameColumn(
                name: "RestrictionId",
                table: "PlansFeatures",
                newName: "FeatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeatureText",
                table: "PlansFeatures",
                newName: "RestrictionText");

            migrationBuilder.RenameColumn(
                name: "FeatureId",
                table: "PlansFeatures",
                newName: "RestrictionId");
        }
    }
}
