using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tp_hospital.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadDoctorId", "Name" },
                values: new object[,]
                {
                    { 1, null, "Cardiologie" },
                    { 2, null, "Urgences" },
                    { 3, null, "Pediatrie" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "DateOfBirth", "Email", "FirstName", "FolderNumber", "LastName" },
                values: new object[,]
                {
                    { 1, "12 rue de la Paix, Paris", new DateTime(1985, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "jean.dupont@email.com", "Jean", 1001, "Dupont" },
                    { 2, "3 avenue des Sciences, Lyon", new DateTime(1990, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "marie.curie@email.com", "Marie", 1002, "Curie" },
                    { 3, "8 boulevard Victor Hugo, Marseille", new DateTime(2000, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "thomas.leroy@email.com", "Thomas", 1003, "Leroy" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "DepartmentId", "FirstName", "LastName", "LicenseNumber", "Specialty" },
                values: new object[,]
                {
                    { 1, 1, "Sophie", "Martin", "LIC-0001", "Cardiologie" },
                    { 2, 2, "Lucas", "Bernard", "LIC-0002", "Medecine generale" },
                    { 3, 3, "Emma", "Dubois", "LIC-0003", "Pediatrie" }
                });

            migrationBuilder.InsertData(
                table: "Consultations",
                columns: new[] { "Id", "AppointmentDate", "DoctorId", "Notes", "PatientId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 21, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, "Bilan cardiaque annuel, RAS.", 1, 2 },
                    { 2, new DateTime(2026, 3, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, "Consultation fievre.", 2, 0 },
                    { 3, new DateTime(2026, 3, 8, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, "Suivi pediatrique.", 3, 0 },
                    { 4, new DateTime(2026, 3, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "Controle tension arterielle.", 1, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Consultations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Consultations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Consultations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Consultations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
