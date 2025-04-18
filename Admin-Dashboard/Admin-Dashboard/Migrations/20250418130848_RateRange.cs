using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admin_Dashboard.Migrations
{
    /// <inheritdoc />
    public partial class RateRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Artisans_Users_Id",
                table: "Artisans",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artisans_Users_Id",
                table: "Artisans");
        }
    }
}
