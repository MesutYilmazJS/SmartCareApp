using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCareApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PortalAuthAndProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Patients",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CareStatus",
                table: "Patients",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByDoctorId",
                table: "Patients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Patients",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageFileName",
                table: "Patients",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Doctors",
                type: "TEXT",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageFileName",
                table: "Doctors",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Doctors",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AdminAccessCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccessCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAccessCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AccessCode",
                table: "Patients",
                column: "AccessCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CreatedByDoctorId",
                table: "Patients",
                column: "CreatedByDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_NationalId",
                table: "Patients",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_Username",
                table: "Doctors",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_CreatedByDoctorId",
                table: "Patients",
                column: "CreatedByDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_CreatedByDoctorId",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "AdminAccessCodes");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AccessCode",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_CreatedByDoctorId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_NationalId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_Username",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CareStatus",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedByDoctorId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ProfileImageFileName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ProfileImageFileName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Doctors");
        }
    }
}
