using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tp_hospital.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedModeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Patients",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Patients",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_City",
                table: "Departments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_Country",
                table: "Departments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_PostalCode",
                table: "Departments",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress_Street",
                table: "Departments",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentDepartmentId",
                table: "Departments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalStaff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StaffType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Function = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Service = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Grade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalStaff", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ParentDepartmentId", "ContactAddress_City", "ContactAddress_Country", "ContactAddress_PostalCode", "ContactAddress_Street" },
                values: new object[] { null, "Paris", "France", "75001", "1 rue du Coeur" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ParentDepartmentId", "ContactAddress_City", "ContactAddress_Country", "ContactAddress_PostalCode", "ContactAddress_Street" },
                values: new object[] { null, "Paris", "France", "75002", "2 avenue des Urgences" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ParentDepartmentId", "ContactAddress_City", "ContactAddress_Country", "ContactAddress_PostalCode", "ContactAddress_Street" },
                values: new object[] { null, "Paris", "France", "75003", "3 rue des Enfants" });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadDoctorId", "Name", "ParentDepartmentId", "ContactAddress_City", "ContactAddress_Country", "ContactAddress_PostalCode", "ContactAddress_Street" },
                values: new object[,]
                {
                    { 4, null, "Cardiologie adulte", 1, "", "France", "", "" },
                    { 5, null, "Cardiologie pediatrique", 1, "", "France", "", "" }
                });

            migrationBuilder.InsertData(
                table: "MedicalStaff",
                columns: new[] { "Id", "FirstName", "HireDate", "LastName", "LicenseNumber", "Salary", "Specialty", "StaffType" },
                values: new object[,]
                {
                    { 1, "Sophie", new DateTime(2015, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martin", "EMP-0001", 6500m, "Cardiologie", "Doctor" },
                    { 2, "Lucas", new DateTime(2018, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bernard", "EMP-0002", 6200m, "Medecine generale", "Doctor" },
                    { 3, "Emma", new DateTime(2020, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dubois", "EMP-0003", 6300m, "Pediatrie", "Doctor" }
                });

            migrationBuilder.InsertData(
                table: "MedicalStaff",
                columns: new[] { "Id", "FirstName", "Grade", "HireDate", "LastName", "Salary", "Service", "StaffType" },
                values: new object[,]
                {
                    { 4, "Claire", "IDE", new DateTime(2019, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Morel", 2800m, "Cardiologie", "Nurse" },
                    { 5, "Pierre", "IADE", new DateTime(2021, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Renard", 3100m, "Urgences", "Nurse" }
                });

            migrationBuilder.InsertData(
                table: "MedicalStaff",
                columns: new[] { "Id", "FirstName", "Function", "HireDate", "LastName", "Salary", "StaffType" },
                values: new object[] { 6, "Nathalie", "Secretaire medicale", new DateTime(2017, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petit", 2200m, "Admin" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address_City", "Address_Country", "Address_PostalCode", "Address_Street" },
                values: new object[] { "Paris", "France", "75001", "12 rue de la Paix" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address_City", "Address_Country", "Address_PostalCode", "Address_Street" },
                values: new object[] { "Lyon", "France", "69001", "3 avenue des Sciences" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address_City", "Address_Country", "Address_PostalCode", "Address_Street" },
                values: new object[] { "Marseille", "France", "13001", "8 boulevard Victor Hugo" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalStaff_LicenseNumber",
                table: "MedicalStaff",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "MedicalStaff");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ContactAddress_City",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_Country",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_PostalCode",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ContactAddress_Street",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ParentDepartmentId",
                table: "Departments");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Address",
                value: "12 rue de la Paix, Paris");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                column: "Address",
                value: "3 avenue des Sciences, Lyon");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3,
                column: "Address",
                value: "8 boulevard Victor Hugo, Marseille");
        }
    }
}
