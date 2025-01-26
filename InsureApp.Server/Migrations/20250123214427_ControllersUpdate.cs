using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class ControllersUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InsuranceTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "InsuranceAgents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "InsuranceAgents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "EndUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EndUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceTypes_Name",
                table: "InsuranceTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAgents_Email",
                table: "InsuranceAgents",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAgents_Username",
                table: "InsuranceAgents",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EndUsers_Email",
                table: "EndUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EndUsers_Username",
                table: "EndUsers",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InsuranceTypes_Name",
                table: "InsuranceTypes");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAgents_Email",
                table: "InsuranceAgents");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAgents_Username",
                table: "InsuranceAgents");

            migrationBuilder.DropIndex(
                name: "IX_EndUsers_Email",
                table: "EndUsers");

            migrationBuilder.DropIndex(
                name: "IX_EndUsers_Username",
                table: "EndUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InsuranceTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "InsuranceAgents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "InsuranceAgents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "EndUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EndUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
