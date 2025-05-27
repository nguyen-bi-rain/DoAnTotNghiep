using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthJWT.Migrations
{
    /// <inheritdoc />
    public partial class updatetableCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_CancellationReasons_CancellationReasonId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CancellationReasonId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CancellationReasonId",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CancellationReasonId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CancellationReasonId",
                table: "Bookings",
                column: "CancellationReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_CancellationReasons_CancellationReasonId",
                table: "Bookings",
                column: "CancellationReasonId",
                principalTable: "CancellationReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
