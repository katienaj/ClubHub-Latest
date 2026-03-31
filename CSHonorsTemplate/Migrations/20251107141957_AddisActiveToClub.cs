using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSHonorsTemplate.Migrations
{
    /// <inheritdoc />
    public partial class AddisActiveToClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Clubs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Clubs");
        }
    }
}
