using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tp_hospital.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "DepartmentId", "FirstName", "LastName", "LicenseNumber", "Specialty" },
                values: new object[,]
                {
                    { 4, 1, "Paul", "Lefebvre", "LIC-0004", "Cardiologie" },
                    { 5, 2, "Julie", "Moreau", "LIC-0005", "Urgences" },
                    { 6, 3, "Antoine", "Girard", "LIC-0006", "Pediatrie" },
                    { 7, 1, "Camille", "Laurent", "LIC-0007", "Cardiologie" },
                    { 8, 2, "Maxime", "Fontaine", "LIC-0008", "Urgences" },
                    { 9, 3, "Isabelle", "Roux", "LIC-0009", "Pediatrie" },
                    { 10, 1, "Nicolas", "Mercier", "LIC-0010", "Cardiologie" }
                });

            migrationBuilder.InsertData(
                table: "MedicalStaff",
                columns: new[] { "Id", "FirstName", "HireDate", "LastName", "LicenseNumber", "Salary", "Specialty", "StaffType" },
                values: new object[,]
                {
                    { 7, "Paul", new DateTime(2016, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lefebvre", "EMP-0004", 6400m, "Cardiologie", "Doctor" },
                    { 8, "Julie", new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Moreau", "EMP-0005", 6100m, "Urgences", "Doctor" },
                    { 9, "Antoine", new DateTime(2021, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Girard", "EMP-0006", 6000m, "Pediatrie", "Doctor" },
                    { 10, "Camille", new DateTime(2017, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Laurent", "EMP-0007", 6350m, "Cardiologie", "Doctor" },
                    { 11, "Maxime", new DateTime(2022, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fontaine", "EMP-0008", 5900m, "Urgences", "Doctor" },
                    { 12, "Isabelle", new DateTime(2019, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Roux", "EMP-0009", 6150m, "Pediatrie", "Doctor" },
                    { 13, "Nicolas", new DateTime(2014, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mercier", "EMP-0010", 6700m, "Cardiologie", "Doctor" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "FolderNumber", "LastName", "Address_City", "Address_Country", "Address_PostalCode", "Address_Street" },
                values: new object[,]
                {
                    { 4, new DateTime(1978, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "alice.bernot@email.com", "Alice", 1004, "Bernot", "Bordeaux", "France", "33000", "5 rue des Lilas" },
                    { 5, new DateTime(1965, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "pierre.faure@email.com", "Pierre", 1005, "Faure", "Toulouse", "France", "31000", "22 avenue Foch" },
                    { 6, new DateTime(1995, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "lucie.garnier@email.com", "Lucie", 1006, "Garnier", "Lille", "France", "59000", "14 rue Nationale" },
                    { 7, new DateTime(1982, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "marc.simon@email.com", "Marc", 1007, "Simon", "Nantes", "France", "44000", "7 impasse des Roses" },
                    { 8, new DateTime(1971, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "isabelle.henry@email.com", "Isabelle", 1008, "Henry", "Strasbourg", "France", "67000", "30 rue du Moulin" },
                    { 9, new DateTime(2003, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "david.blanc@email.com", "David", 1009, "Blanc", "Montpellier", "France", "34000", "18 chemin des Vignes" },
                    { 10, new DateTime(1988, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "catherine.richard@email.com", "Catherine", 1010, "Richard", "Rennes", "France", "35000", "2 place de la Republique" },
                    { 11, new DateTime(1993, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "nicolas.lambert@email.com", "Nicolas", 1011, "Lambert", "Nice", "France", "06000", "9 rue Saint-Jacques" },
                    { 12, new DateTime(1979, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "sophie.dumont@email.com", "Sophie", 1012, "Dumont", "Grenoble", "France", "38000", "45 boulevard Gambetta" },
                    { 13, new DateTime(2008, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "julien.dupuis@email.com", "Julien", 1013, "Dupuis", "Dijon", "France", "21000", "3 allee des Peupliers" },
                    { 14, new DateTime(1960, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "claire.bertrand@email.com", "Claire", 1014, "Bertrand", "Reims", "France", "51100", "11 rue du Four" },
                    { 15, new DateTime(1975, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "antoine.morin@email.com", "Antoine", 1015, "Morin", "Clermont-Fd", "France", "63000", "60 avenue Jean Jaures" },
                    { 16, new DateTime(1998, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "marine.rousseau@email.com", "Marine", 1016, "Rousseau", "Rouen", "France", "76000", "28 rue des Capucines" },
                    { 17, new DateTime(1956, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "francois.legrand@email.com", "Francois", 1017, "Legrand", "Toulouse", "France", "31000", "6 place du Capitole" },
                    { 18, new DateTime(2001, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "laura.petit@email.com", "Laura", 1018, "Petit", "Metz", "France", "57000", "15 rue Pasteur" },
                    { 19, new DateTime(1987, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "romain.chevalier@email.com", "Romain", 1019, "Chevalier", "Tours", "France", "37000", "33 avenue de la Gare" },
                    { 20, new DateTime(1992, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "nadia.bonnet@email.com", "Nadia", 1020, "Bonnet", "Caen", "France", "14000", "8 rue des Hirondelles" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MedicalStaff",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
