using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASHOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addPublicIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainImagePublicId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainImagePublicId",
                table: "Products");
        }
    }
}
