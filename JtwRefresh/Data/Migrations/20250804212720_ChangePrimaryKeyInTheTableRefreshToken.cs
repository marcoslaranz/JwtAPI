using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JtwRefresh.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePrimaryKeyInTheTableRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RefreshTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "RefreshTokens");
        }
    }
}
