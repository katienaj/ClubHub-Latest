using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSHonorsTemplate.Migrations
{
    /// <inheritdoc />
    public partial class PersonInAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendance_PersonId",
                table: "Attendance",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_People_PersonId",
                table: "Attendance",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_People_PersonId",
                table: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_PersonId",
                table: "Attendance");
        }
    }
}
