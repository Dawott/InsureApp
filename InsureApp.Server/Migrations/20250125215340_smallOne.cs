using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class smallOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "InsuranceAgents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "InsuranceAgents");
        }
    }
}
