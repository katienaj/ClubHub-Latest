using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSHonorsTemplate.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Joins_Clubs_ClubID",
                table: "Joins");

            migrationBuilder.DropForeignKey(
                name: "FK_Joins_People_PersonID",
                table: "Joins");

            migrationBuilder.DropColumn(
                name: "CampusID",
                table: "Joins");

            migrationBuilder.RenameColumn(
                name: "CampusId",
                table: "UserPermissions",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "People",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "Joins",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "ClubID",
                table: "Joins",
                newName: "ClubId");

            migrationBuilder.RenameColumn(
                name: "JoinID",
                table: "Joins",
                newName: "JoinId");

            migrationBuilder.RenameIndex(
                name: "IX_Joins_PersonID",
                table: "Joins",
                newName: "IX_Joins_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Joins_ClubID",
                table: "Joins",
                newName: "IX_Joins_ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Joins_Clubs_ClubId",
                table: "Joins",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Joins_People_PersonId",
                table: "Joins",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Joins_Clubs_ClubId",
                table: "Joins");

            migrationBuilder.DropForeignKey(
                name: "FK_Joins_People_PersonId",
                table: "Joins");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "UserPermissions",
                newName: "CampusId");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "People",
                newName: "PersonID");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "Joins",
                newName: "PersonID");

            migrationBuilder.RenameColumn(
                name: "ClubId",
                table: "Joins",
                newName: "ClubID");

            migrationBuilder.RenameColumn(
                name: "JoinId",
                table: "Joins",
                newName: "JoinID");

            migrationBuilder.RenameIndex(
                name: "IX_Joins_PersonId",
                table: "Joins",
                newName: "IX_Joins_PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_Joins_ClubId",
                table: "Joins",
                newName: "IX_Joins_ClubID");

            migrationBuilder.AddColumn<int>(
                name: "CampusID",
                table: "Joins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Joins_Clubs_ClubID",
                table: "Joins",
                column: "ClubID",
                principalTable: "Clubs",
                principalColumn: "ClubId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Joins_People_PersonID",
                table: "Joins",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
