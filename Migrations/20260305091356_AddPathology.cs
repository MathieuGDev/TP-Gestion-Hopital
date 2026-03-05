using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tp_hospital.Migrations
{
    /// <inheritdoc />
    public partial class AddPathology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pathologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pathologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientPathology",
                columns: table => new
                {
                    PathologyId = table.Column<int>(type: "INTEGER", nullable: false),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientPathology", x => new { x.PathologyId, x.PatientId });
                    table.ForeignKey(
                        name: "FK_PatientPathology_Pathologies_PathologyId",
                        column: x => x.PathologyId,
                        principalTable: "Pathologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientPathology_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Pathologies",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Pression arterielle chroniquement elevee", "Hypertension arterielle" },
                    { 2, "Trouble metabolique du glucose", "Diabete de type 2" },
                    { 3, "Maladie inflammatoire chronique des voies respiratoires", "Asthme" },
                    { 4, "Incapacite du coeur a pomper suffisamment de sang", "Insuffisance cardiaque" },
                    { 5, "Degenerescence du cartilage articulaire", "Arthrose" }
                });

            migrationBuilder.InsertData(
                table: "PatientPathology",
                columns: new[] { "PathologyId", "PatientId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 17 },
                    { 2, 4 },
                    { 2, 8 },
                    { 3, 2 },
                    { 3, 10 },
                    { 4, 1 },
                    { 4, 14 },
                    { 5, 5 },
                    { 5, 14 },
                    { 5, 17 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pathologies_Name",
                table: "Pathologies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientPathology_PatientId",
                table: "PatientPathology",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientPathology");

            migrationBuilder.DropTable(
                name: "Pathologies");
        }
    }
}
