using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSHonorsTemplate.Migrations
{
    /// <inheritdoc />
    public partial class AdvisorInCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs");

            migrationBuilder.AlterColumn<int>(
                name: "ClubTypeId",
                table: "Clubs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs",
                column: "ClubTypeId",
                principalTable: "ClubType",
                principalColumn: "ClubTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs");

            migrationBuilder.AlterColumn<int>(
                name: "ClubTypeId",
                table: "Clubs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs",
                column: "ClubTypeId",
                principalTable: "ClubType",
                principalColumn: "ClubTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
