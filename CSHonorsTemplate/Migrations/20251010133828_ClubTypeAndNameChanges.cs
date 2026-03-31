using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSHonorsTemplate.Migrations
{
    /// <inheritdoc />
    public partial class ClubTypeAndNameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Info",
                table: "Clubs",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Clubs",
                newName: "VeracrossId");

            migrationBuilder.AddColumn<int>(
                name: "ClubTypeId",
                table: "Clubs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClubType",
                columns: table => new
                {
                    ClubTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubType", x => x.ClubTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_ClubTypeId",
                table: "Clubs",
                column: "ClubTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs",
                column: "ClubTypeId",
                principalTable: "ClubType",
                principalColumn: "ClubTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_ClubType_ClubTypeId",
                table: "Clubs");

            migrationBuilder.DropTable(
                name: "ClubType");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_ClubTypeId",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "ClubTypeId",
                table: "Clubs");

            migrationBuilder.RenameColumn(
                name: "VeracrossId",
                table: "Clubs",
                newName: "ExternalId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Clubs",
                newName: "Info");
        }
    }
}
