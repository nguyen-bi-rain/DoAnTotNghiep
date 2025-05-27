using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthJWT.Migrations
{
    /// <inheritdoc />
    public partial class addtypeforconvenience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Conveniences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Conveniences");
        }
    }
}
