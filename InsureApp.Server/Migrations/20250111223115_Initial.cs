using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Licence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndUserId = table.Column<int>(type: "int", nullable: false),
                    InsuranceAgentId = table.Column<int>(type: "int", nullable: true),
                    InsuranceTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceReports_EndUsers_EndUserId",
                        column: x => x.EndUserId,
                        principalTable: "EndUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsuranceReports_InsuranceAgents_InsuranceAgentId",
                        column: x => x.InsuranceAgentId,
                        principalTable: "InsuranceAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InsuranceReports_InsuranceTypes_InsuranceTypeId",
                        column: x => x.InsuranceTypeId,
                        principalTable: "InsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InsuranceReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceDocuments_InsuranceReports_InsuranceReportId",
                        column: x => x.InsuranceReportId,
                        principalTable: "InsuranceReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_InsuranceReportId",
                table: "InsuranceDocuments",
                column: "InsuranceReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceReports_EndUserId",
                table: "InsuranceReports",
                column: "EndUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceReports_InsuranceAgentId",
                table: "InsuranceReports",
                column: "InsuranceAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceReports_InsuranceTypeId",
                table: "InsuranceReports",
                column: "InsuranceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceDocuments");

            migrationBuilder.DropTable(
                name: "InsuranceReports");

            migrationBuilder.DropTable(
                name: "EndUsers");

            migrationBuilder.DropTable(
                name: "InsuranceAgents");

            migrationBuilder.DropTable(
                name: "InsuranceTypes");
        }
    }
}
